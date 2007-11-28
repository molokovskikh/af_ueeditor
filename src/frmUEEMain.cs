using System;
using System.IO;
using System.Globalization;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;
using System.Reflection;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using System.Threading;
using System.Security.Permissions;
using Microsoft.Win32;
using System.Diagnostics;



[assembly: RegistryPermissionAttribute(SecurityAction.RequestMinimum, ViewAndModify = "HKEY_CURRENT_USER")]
namespace UEEditor
{

	[FlagsAttribute]
	public enum FormMask : byte
	{
		NameForm = 1,
		FirmForm = 2,
		CurrForm = 4,
		MarkForb = 8
	}
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public partial class frmUEEMain : System.Windows.Forms.Form
	{

		private MySqlConnection MyCn = new MySqlConnection();
		private MySqlCommand MyCmd = new MySqlCommand();
		private MySqlDataAdapter MyDA = new MySqlDataAdapter();
		private MySqlDataAdapter daJobs;

		private string BaseRegKey = "Software\\Inforoom\\UEEditor";
		private string JregKey;
		private string CregKey;
		private string UEregKey;
		private string FregKey;
		private string ZregKey;
        
		public string PriceFMT = String.Empty;
        public string FileExt = String.Empty;
		public long LockedPriceCode = -1;
		public long LockedSynonym = -1;
		public frmProgress f = null;
		public int SynonymCount = 0;
		public int HideSynonymCount = 0;
		public int DuplicateSynonymCount = 0;
		public int SynonymFirmCrCount = 0;
		public int ForbiddenCount = 0;

		public frmUEEMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		private void frmUEEMain_Load(object sender, EventArgs e)
		{
			//
			try
			{
				LoadColor(btnJobsBlock, btnJobsBlock.BackColor.ToArgb());
				LoadColor(btnJobsNamePos, btnJobsNamePos.BackColor.ToArgb());
				LoadColor(btnJobs50, btnJobs50.BackColor.ToArgb());

				JregKey = BaseRegKey + "\\J";
				CregKey = BaseRegKey + "\\C";
				UEregKey = BaseRegKey + "\\UE";
				FregKey = BaseRegKey + "\\F";
				ZregKey = BaseRegKey + "\\Z";
				JobsGridControl.MainView.RestoreLayoutFromRegistry(JregKey);
				CatalogGridControl.MainView.RestoreLayoutFromRegistry(CregKey);
				UnrecExpGridControl.MainView.RestoreLayoutFromRegistry(UEregKey);
				ForbGridControl.MainView.RestoreLayoutFromRegistry(FregKey);
				ZeroGridControl.MainView.RestoreLayoutFromRegistry(ZregKey);
			}
			catch
			{
			}

			MyCn.ConnectionString = "server=sql.analit.net; user id=system; password=123; database=farm;convert zero datetime=true;";
#if DEBUG
			MyCn.ConnectionString = "server=testsql.analit.net; user id=system; password=newpass; database=farm;convert zero datetime=true;";
#endif
			MyCn.Open();
			MyDA = new MySqlDataAdapter(MyCmd);
			MyCmd.Connection = MyCn;

			tcMain.TabPages.Remove(tpUnrecExp);
			tcMain.TabPages.Remove(tpZero);
			tcMain.TabPages.Remove(tpForb);

			//Создали ДатаАдаптер для таблицы заданий
			DAJobsCreate();

			//Заполняем таблицу заданий
			JobsGridFill();

			//Запоняем каталожные таблицы
			CatalogFirmCrGridFill(MyCmd, MyDA);

			CatalogNameGridFill(MyCmd, MyDA);

			FormGridFill(MyCmd, MyDA);

			//
			JobsGridControl.Select();
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			UEEditorExceptionHandler feh = new UEEditorExceptionHandler();

			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(feh.OnThreadException);

			Application.Run(new frmUEEMain());	
		}

		private void SaveTableStyle(StreamWriter sr, GridControl dt)
		{
			foreach(GridColumn gc in ((GridView)dt.DefaultView).Columns)
			{
				sr.WriteLine(gc.Width);
			}
		}

		private void LoadTableStyle(StreamReader sr, GridControl dt)
		{
			string line;
			foreach(GridColumn dc in ((GridView)dt.DefaultView).Columns)
			{
				line=sr.ReadLine();
				dc.Width = Convert.ToInt32(line);
			}
		}

		private void frmUEEMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (tcMain.SelectedTab == tpUnrecExp)
			{
				DialogResult DRes;
				DRes = MessageBox.Show("Вы находитесь в режиме редактирования прайс-листа. Сохранить изменения?", "Вопрос", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
				UnlockJob(DRes);
				if (DRes == DialogResult.Cancel)
				{
					e.Cancel = true;
					return;
				}
			}
			SaveColor(btnJobsBlock);
			SaveColor(btnJobsNamePos);
			SaveColor(btnJobs50);

			JobsGridControl.MainView.SaveLayoutToRegistry(JregKey);
			CatalogGridControl.MainView.SaveLayoutToRegistry(CregKey);
			UnrecExpGridControl.MainView.SaveLayoutToRegistry(UEregKey);
			ForbGridControl.MainView.SaveLayoutToRegistry(FregKey);
			ZeroGridControl.MainView.SaveLayoutToRegistry(ZregKey);

			MyCn.Close();
		}

		private void DAJobsCreate()
		{
			daJobs = new MySqlDataAdapter(
				@"
SELECT  PD.FirmCode as JFirmCode,
        cd.ShortName as FirmShortName,
        PD.PriceCode                                                                                                         As JPriceCode,
        concat(CD.ShortName, '(', if(pc.PriceCode = pc.ShowPriceCode, pd.PriceName, concat('[Колонка] ', pc.CostName)), ')') as JName,
        regions.region                                                                                                       As JRegion,
        pui.DateCurPrice                                                                                               AS JPriceDate,
        FormRules.MaxOld,
        statunrecexp.Pos                                                                        AS JPos,
        statunrecexp.NamePos                                                                    AS JNamePos,
        pui.DateLastForm                                                                        AS JJobDate,
        CD.FirmSegment                                                                          As JWholeSale,
        bp.BlockBy                                                                              As JBlockBy,
        FormRules.ParentSynonym                                                                 as JParentSynonym,
        FormRules.PriceFmt                                                                      As JPriceFMT,
        pfmt.FileExtention                                                                      as JExt,
        if((synonympui.LastSynonymsCreation is not null) and (pui.DateLastForm < synonympui.LastSynonymsCreation), 1, 0) AS JNeedRetrans,
        if(pui.DateLastForm < pui.LastRetrans, 1, 0)                                            AS JRetranced,
        pui.DateLastForm                                                                        AS JDateLastForm,
        if(FormRules.ParentSynonym is null, '', concat(pcd.ShortName, '(', ppd.PriceName, ')')) AS JParentName
FROM
  (usersettings.ClientsData AS CD,
   FormRules,
   PriceFMTs as pfmt,
   usersettings.pricesdata AS PD,
   regions,
   usersettings.pricescosts pc,
   usersettings.price_update_info pui,
   usersettings.price_update_info synonympui,
   (select
      unrecexp.PriceCode,
      count(unrecexp.RowID) as Pos,
      COUNT(IF(unrecexp.TmpProductId=0,unrecexp.TmpProductId,NULL)) as NamePos
    from
      farm.unrecexp
    group by unrecexp.PriceCode) statunrecexp
  )
LEFT JOIN blockedprice bp
ON      bp.PriceCode = PD.PriceCode
LEFT JOIN usersettings.pricesdata ppd
ON      ppd.pricecode = FormRules.ParentSynonym
LEFT JOIN usersettings.clientsdata pcd
ON      pcd.FirmCode       = ppd.firmcode
WHERE   FormRules.firmcode =PD.pricecode
    AND FormRules.PriceFmt = pfmt.Format
    and statunrecexp.PriceCode = PD.pricecode
    and pui.PriceCode = PD.pricecode
    and synonympui.PriceCode = if(FormRules.ParentSynonym is null, PD.pricecode, FormRules.ParentSynonym)
    AND CD.firmcode        =PD.firmcode
    AND regions.regioncode =CD.regioncode
    AND pd.agencyenabled   =1
    AND pc.PriceCode = pd.PriceCode
", 
				MyCn);
		}

		private void JobsGridFill()
		{
			long CurrPriceCode = -1;
			List<long> selectedPrices = new List<long>();
			if (gvJobs.FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow drJ = gvJobs.GetDataRow(gvJobs.FocusedRowHandle);
				if (drJ != null)
					CurrPriceCode = Convert.ToInt64(drJ["JPriceCode"]);
			}

			int[] selected = gvJobs.GetSelectedRows();
			if (selected.Length > 0)
			{
				//выбрали прайс-листы из базы, т.к. может произойти обновление таблицы
				foreach (int rowHandle in selected)
					if (rowHandle != GridControl.InvalidRowHandle)
						selectedPrices.Add((long)gvJobs.GetDataRow(rowHandle)[JPriceCode.ColumnName]);
			}

			dtJobs.Clear();
			
			JobsGridControl.BeginUpdate();
			try
			{
				daJobs.Fill(dtJobs);
			}
			finally
			{
				JobsGridControl.EndUpdate();
			}

			LocateJobs(CurrPriceCode, (selectedPrices.Count <= 1) ? null : selectedPrices);
			statusBar1.Panels[0].Text = "Заданий в очереди: " + dtJobs.Rows.Count;
		}

		private void UnrecExpGridFill(MySqlCommand MyCmd, MySqlDataAdapter MyDA)
		{
			dtUnrecExp.Clear();

			MyCmd.CommandText =
				@"SELECT RowID As UERowID,
                  Name1 As UEName1, 
				  Name2 AS UEName2, 
				  Name3 As UEName3, 
				  FirmCr As UEFirmCr, 
				  CountryCr, 
				  Code As UECode, 
				  CodeCr As UECodeCr, 
				  Unit As UEUnit, 
				  Volume As UEVolume, 
				  Quantity As UEQuantity, 
				  Note, 
				  Period As UEPeriod, 
				  Doc, 
				  BaseCost As UEBaseCost, 
				  Currency As UECurrency, 
				  TmpProductId As UETmpProductId,  
				  TmpCodeFirmCr As UETmpCodeFirmCr, 
				  TmpCurrency, 
				  Status As UEStatus,
                  Already As UEAlready, 
				  Junk As UEJunk,
				  HandMade As UEHandMade
				  FROM farm.UnrecExp 
				  WHERE PriceCode= ?LockedPriceCode ORDER BY Name1";

			MyCmd.Parameters.Clear();
			MyCmd.Parameters.AddWithValue("?LockedPriceCode", LockedPriceCode);
			
			UnrecExpGridControl.BeginUpdate();
			try
			{
				MyDA.Fill(dtUnrecExp);
			}
			finally
			{
				UnrecExpGridControl.EndUpdate();
			}
		}

		private void CatalogNameGridFill(MySqlCommand MyCmd, MySqlDataAdapter MyDA)
		{
			dtCatalogNames.Clear();
			MyCmd.CommandText = @"
SELECT
 distinct cn.Id, cn.Name
from
  catalogs.CatalogNames cn,
  catalogs.catalog cat,
  catalogs.products p
where
    cat.NameId = cn.Id
and cat.Hidden = 0
and p.CatalogId = cat.Id
and p.Hidden = 0
order by Name";
			MyDA.Fill(dtCatalogNames);

		}
		
		private void CatalogFirmCrGridFill(MySqlCommand MyCmd, MySqlDataAdapter MyDA)
		{
			dtCatalogFirmCr.Clear();
			MyCmd.CommandText = 
				@"SELECT 
					CodeFirmCr As CCode, 
					FirmCr As CName 
				FROM CatalogFirmCr
                where CodeFirmCr <> 1 
				Order By FirmCr";

			MyDA.Fill(dtCatalogFirmCr);

		}

		private void FormGridFill(MySqlCommand MyCmd, MySqlDataAdapter MyDA)
		{
			MyCmd.CommandText =
				@"
select
  Catalog.*,
  CatalogForms.Form,
  count(products.id) as productscount
from
  catalogs.Catalog,
  catalogs.CatalogForms,
  catalogs.products
where
    CatalogForms.Id = Catalog.FormId
and Catalog.Hidden = 0
and products.CatalogId = Catalog.id
and products.Hidden = 0
group by Catalog.id
order by Form";
			MyDA.Fill(dtCatalog);
		}

		private void ProductsFill(MySqlCommand MyCmd, MySqlDataAdapter MyDA, ulong CatalogId)
		{
			MyCmd.Parameters.Clear();
			MyCmd.Parameters.AddWithValue("?CatalogId", CatalogId);
			MyCmd.CommandText =
				@"
SELECT
  Products.Id,
  Catalog.Id as CatalogId,
  null as Properties
FROM
(
catalogs.Products,
catalogs.Catalog
)
left join catalogs.ProductProperties on ProductProperties.ProductId = Products.Id
where
    Catalog.Id = Products.CatalogID
and Catalog.Id = ?CatalogId 
and ProductProperties.ProductId is null
and Products.Hidden = 0
union all
SELECT
  Products.Id,
  Catalog.Id as CatalogId,
  GROUP_CONCAT(PropertyValues.Value
    order by Properties.Id, PropertyValues.Id
    SEPARATOR ', '
  ) as Properties
FROM
catalogs.Products,
catalogs.Catalog,
catalogs.CatalogNames,
catalogs.CatalogForms,
catalogs.ProductProperties,
catalogs.PropertyValues,
catalogs.Properties
where
    Catalog.Id = Products.CatalogID
and Catalog.Id = ?CatalogId 
and CatalogNames.Id = Catalog.NameID
and CatalogForms.Id = Catalog.FormID
and ProductProperties.ProductId = Products.Id
and PropertyValues.Id = ProductProperties.PropertyValueId
and Properties.Id = PropertyValues.PropertyId
and Products.Hidden = 0
group by Products.Id
order by Properties
";

			MyDA.Fill(dtProducts);
		}

		private void ProductsFillByProductId(MySqlCommand MyCmd, MySqlDataAdapter MyDA, ulong ProductId)
		{
			MyCmd.Parameters.Clear();
			MyCmd.Parameters.AddWithValue("?ProductId", ProductId);
			MyCmd.CommandText =
				@"
SELECT
  Products.Id,
  Catalog.Id as CatalogId,
  null as Properties
FROM
(
catalogs.Products,
catalogs.Catalog
)
left join catalogs.ProductProperties on ProductProperties.ProductId = Products.Id
where
    Catalog.Id = Products.CatalogID
and Products.Id = ?ProductId 
and ProductProperties.ProductId is null
and Products.Hidden = 0
union all
SELECT
  Products.Id,
  Catalog.Id as CatalogId,
  GROUP_CONCAT(Properties.PropertyName, '=', PropertyValues.Value
    order by Properties.Id, PropertyValues.Id
    SEPARATOR ', '
  ) as Properties
FROM
catalogs.Products,
catalogs.Catalog,
catalogs.CatalogNames,
catalogs.CatalogForms,
catalogs.ProductProperties,
catalogs.PropertyValues,
catalogs.Properties
where
    Catalog.Id = Products.CatalogID
and Products.Id = ?ProductId 
and CatalogNames.Id = Catalog.NameID
and CatalogForms.Id = Catalog.FormID
and ProductProperties.ProductId = Products.Id
and PropertyValues.Id = ProductProperties.PropertyValueId
and Properties.Id = PropertyValues.PropertyId
and Products.Hidden = 0
group by Products.Id
";

			MyDA.Fill(dtProducts);
		}

		private void UpdateCatalog()
		{
			CatalogGridControl.BeginUpdate();
			try
			{
				dtProducts.Clear();
				dtCatalog.Clear();

				CatalogFirmCrGridFill(MyCmd, MyDA);

				CatalogNameGridFill(MyCmd, MyDA);

				FormGridFill(MyCmd, MyDA);
			}
			finally
			{
				CatalogGridControl.EndUpdate();
			}
		}

		private void btnDelJob_Click(object sender, System.EventArgs e)
		{
			int[] selected = gvJobs.GetSelectedRows();

			if (selected.Length > 0)
			{
				if (MessageBox.Show("Вы действительно хотите удалить выбранные задания?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					List<long> selectedPrices = new List<long>();

					//выбрали прайс-листы из базы, т.к. может произойти обновление таблицы
					foreach (int rowHandle in selected)
						if (rowHandle != GridControl.InvalidRowHandle)
							selectedPrices.Add((long)gvJobs.GetDataRow(rowHandle)[JPriceCode.ColumnName]);

					//удаляем задания
					foreach (long selectedPrice in selectedPrices)
					{
						MySqlTransaction tran = MyCn.BeginTransaction();
						try
						{
							MySqlCommand cmdDeleteJob = new MySqlCommand(@"
DELETE FROM 
  farm.UnrecExp
WHERE 
    PriceCode = ?PriceCode
AND not exists(select * from blockedprice bp where bp.PriceCode = UnrecExp.PriceCode)",
								MyCn, tran);
							cmdDeleteJob.Parameters.AddWithValue("?PriceCode", selectedPrice);
							cmdDeleteJob.ExecuteNonQuery();
							tran.Commit();
						}
						catch
						{ 
							if (tran != null)
								try
								{
									tran.Rollback();
								}
								catch { }
							throw;
						}
					}

					JobsGridFill();
				}
			}
		}
		
		private void JobsGridControl_DoubleClick(object sender, System.EventArgs e)
		{
			if (gvJobs.FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvJobs.GetDataRow(gvJobs.FocusedRowHandle);
				if (dr[colJBlockBy.FieldName].ToString() == String.Empty || dr[colJBlockBy.FieldName].ToString() == Environment.UserName)
					LockJob();
			}
		}

		private void tcMain_SelectedIndexChanged(object sender, System.EventArgs e)
		{	
			if (tcMain.SelectedTab == tpJobs)
			{
				JobsGridControl.Select();
			}

			if (tcMain.SelectedTab == tpUnrecExp)
				UnrecExpGridControl.Select();

			if (tcMain.SelectedTab == tpForb)
			{
				ForbGridControl.Select();
				dtForb.Clear();

				MyCmd.CommandText = 
					@"SELECT Forb As FForb 
					            FROM farm.Forb 
					            WHERE PriceCode= ?JPriceCode";
				MyCmd.Parameters.Clear();
				MyCmd.Parameters.AddWithValue("?JPriceCode", LockedPriceCode);

				MyDA.Fill(dtForb);

			}

			if (tcMain.SelectedTab == tpZero)
			{
				ZeroGridControl.Select();

				dsMain.Tables["ZeroGrid"].Clear();

				MyCmd.CommandText = 
					@"SELECT 
									Code As ZCode, 
									CodeCr As ZCodeCr, 
									Name As ZName, 
									FirmCr As ZFirmCr, 
									Currency As ZCurrency, 
									Unit As ZUnit, 
									Volume AS ZVolume, 
									Quantity As ZQuantity, 
									Period As ZPeriod
								FROM farm.Zero 
								WHERE PriceCode= ?JPriceCode";

				MyCmd.Parameters.Clear();
				MyCmd.Parameters.AddWithValue("?JPriceCode", LockedPriceCode);
				MyDA.Fill(dtZero);
			}

     	}

		private FormMask GetMask(int NumRow, string FieldName)
		{
			DataRow dr=gvUnrecExp.GetDataRow(NumRow);
			FormMask mask = (FormMask)Convert.ToByte(dr[FieldName]);
			return mask;
		}

		private bool NotNameForm(int NumRow, string FieldName)
		{
			FormMask m = GetMask(NumRow, FieldName);
			return (m & FormMask.NameForm) != FormMask.NameForm;
		}

		private bool NotFirmForm(int NumRow, string FieldName)
		{
			FormMask m = GetMask(NumRow, FieldName);
			return (m & FormMask.FirmForm) != FormMask.FirmForm;
		}

		private bool NotCurrForm(int NumRow, string FieldName)
		{
			FormMask m = GetMask(NumRow, FieldName);
			return (m & FormMask.CurrForm) != FormMask.CurrForm;
		}

		private bool MarkForbidden(int NumRow, string FieldName)
		{
			FormMask m = GetMask(NumRow, FieldName);
			return (m & FormMask.MarkForb) == FormMask.MarkForb;
		}

		private string GetFilterString(string Value, string FieldName)
		{
			int FirstLen = 4;
			string[] flt = Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			ArrayList newflt = new ArrayList();
			for(int i=0;i<flt.Length;i++)
			{
				if (flt[i].Length >= 3)
				{
					if (flt[i].Length >= FirstLen)
						newflt.Add(PrepareArg(flt[i].Substring(0, FirstLen).Replace("'", "''")));
					else
						newflt.Add(PrepareArg(flt[i].Replace("'", "''")));
				}
			}
			string[] flt2 = new string[newflt.Count];
			newflt.CopyTo(flt2);
			return "[" + FieldName + "] like '" + String.Join("%' or [" + FieldName + "] like '", flt2) + "%'";
		}

		private void GotoCatalogPosition(GridView selected, string Value, string FieldName)
		{
			int WordLen = 3;
			string[] flt = Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			List<string> firstChars = new List<string>();
			for (int i = 0; i < flt.Length; i++)
			{
				if (flt[i].Length >= WordLen)
					firstChars.Add(flt[i].Substring(0, WordLen));
			}

			int positionId = 0, maxCompareCount = 0;

			//Произодим поиск
			for (int i = 0; i < selected.DataRowCount; i++)
			{
				string PropertiesValue = selected.GetDataRow(i)[FieldName].ToString();
				int compareCount = 0;
				foreach (string s in firstChars)
					if (PropertiesValue.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0)
						compareCount++;

				if (compareCount > maxCompareCount)
				{
					maxCompareCount = compareCount;
					positionId = i;
				}
			}

			if (positionId != 0)
				selected.FocusedRowHandle = positionId;
		}

		private void GotoProductPosition(GridView selected, string Value, string FieldName)
		{
			int WordLen = 3;
			string[] flt = Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			List<string> firstChars = new List<string>();
			for (int i = 0; i < flt.Length; i++)
			{
				if (flt[i].Length >= WordLen)
					firstChars.Add(flt[i].Substring(0, WordLen));
			}

			int positionId = 0, maxCompareCount = 0;

			//Произодим поиск со второй записи, т.к. в первый находится "чистый" продукт (без свойств)
			for (int i = 1; i < selected.DataRowCount; i++)
			{
				string PropertiesValue = selected.GetDataRow(i)[FieldName].ToString();
				int compareCount = 0;
				foreach (string s in firstChars)
					if (PropertiesValue.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0)
						compareCount++;

				if (compareCount > maxCompareCount)
				{
					maxCompareCount = compareCount;
					positionId = i;
				}
			}

			if (positionId != 0)
				selected.FocusedRowHandle = positionId;
		}

		private void ShowCatalog(int FocusedRowHandle)
		{
			DataRow dr=gvUnrecExp.GetDataRow(FocusedRowHandle);
			grpBoxCatalog2.Text = "Каталог товаров";
			CatalogGridControl.Visible = true;
			gcFirmCr.Visible = false;

			CatalogGridControl.FocusedView = gvCatalog;
			gvCatalog.CollapseAllDetails();
			gvCatalog.ZoomView();
			gvCatalog.ActiveFilter.Clear();
			gvCatalog.ActiveFilter.Add(gvCatalog.Columns["Name"], new ColumnFilterInfo( GetFilterString( GetFullUnrecName(FocusedRowHandle), "Name" ) , ""));
			if (gvCatalog.DataRowCount == 0)
				gvCatalog.ActiveFilter.Clear();
			else
				GotoCatalogPosition(gvCatalog, GetFullUnrecName(FocusedRowHandle), "Name");
		}

		private void ShowCatalogFirmCr(int FocusedRowHandle)
		{
			DataRow dr=gvUnrecExp.GetDataRow(FocusedRowHandle);
			grpBoxCatalog2.Text = "Каталог фирм производителей";
			gcFirmCr.Visible = true;
			CatalogGridControl.Visible = false;
			
			if (dr["UEFirmCr"].ToString() != String.Empty)
			{
				gvFirmCr.ActiveFilter.Clear();
				gvFirmCr.ActiveFilter.Add(gvFirmCr.Columns["CName"], new ColumnFilterInfo(GetFilterString(dr["UEFirmCr"].ToString(), "CName"), ""));
				if (gvFirmCr.DataRowCount == 0)
					gvFirmCr.ActiveFilter.Clear();
				else
					GotoCatalogPosition(gvFirmCr, dr["UEFirmCr"].ToString(), "CName");
			}
			else
			{
				gvFirmCr.ActiveFilter.Clear();
			}
		}

		private void MoveToCatalog()
		{
			if (gvUnrecExp.FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr=gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);
				
				if ((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.MarkForb) == FormMask.MarkForb)
				{
					CatalogGridControl.Enabled = false;
					ClearCatalogGrid();
				}
				else
					if ((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.NameForm) != FormMask.NameForm)
				{
					CatalogGridControl.Enabled = true;
					ShowCatalog(gvUnrecExp.FocusedRowHandle);
					CatalogGridControl.Focus();
				}
				else
					if ((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.FirmForm) != FormMask.FirmForm)
				{
					gcFirmCr.Enabled = true;
					ShowCatalogFirmCr(gvUnrecExp.FocusedRowHandle);
					gcFirmCr.Focus();
				}
				else
					if( 7 == (int)dr["UEStatus"])
				{
					CatalogGridControl.Enabled = false;
					ClearCatalogGrid();
				}
			}
		}

		private void UnrecExpGridControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (gvUnrecExp.FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				bool flag = false;
				DataRow UEdr = gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);

				if (e.KeyCode == Keys.Enter)
					MoveToCatalog();

				if (e.KeyCode == Keys.Escape)
					gvUnrecExp.FocusedRowHandle += 1;

				if (e.KeyCode == Keys.Back)
				{
					if ((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.MarkForb) == FormMask.MarkForb)
						if(MessageBox.Show("Отменить запрещение?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
						{
							UnmarkUnrecExpAsForbidden(gvUnrecExp.FocusedRowHandle);
							return;
						}
						else
							return;

					if ((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.NameForm) == FormMask.NameForm)
					{
						DataRow drUN = gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);
						if (drUN != null)
						{
							dtProducts.Clear();
							ProductsFillByProductId(MyCmd, MyDA, Convert.ToUInt64(drUN[UETmpProductId]));

							DataRow[] drProducts = dtProducts.Select("Id = " + drUN[UETmpProductId].ToString());

							if (drProducts.Length > 0)
							{
								DataRow drCatalog = drProducts[0].GetParentRow("Products");
								DataRow drCatalogName = drCatalog.GetParentRow("CatalogNames");
								string Mess = String.Format("Наименование: {0}\r\nФорма: {1}\r\nОтменить сопоставление по наименованию?", drCatalogName[colCatalogNameName], drCatalog[colCatalogForm]);
								if (MessageBox.Show(Mess, "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
								{
									UnmarkUnrecExpAsNameForm(gvUnrecExp.FocusedRowHandle);
									SetReserved(false, gvUnrecExp.FocusedRowHandle);
									flag = true;
								}
							}
						}
					}

					if ((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.FirmForm) == FormMask.FirmForm)
					{
						DataRow drUN = gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);
						if (drUN != null)
						{
							string FirmName = String.Empty;
							if ((Int64)drUN[UETmpCodeFirmCr] == 1)
								FirmName = "-";
							else
							{
								DataRow[] drFM = dtCatalogFirmCr.Select("CCode = " + drUN[UETmpCodeFirmCr].ToString());
								if (drFM.Length > 0)
								{
									FirmName = drFM[0]["CName"].ToString();
								}
							}
							if(FirmName != String.Empty && MessageBox.Show("Производитель: " + FirmName+ "\r\nОтменить сопоставление по производителю?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
							{
								UnmarkUnrecExpAsFirmForm(gvUnrecExp.FocusedRowHandle);
								flag = true;
							}
						}
					}

					if (flag)
						MoveToCatalog();
				}

				if (e.KeyCode == Keys.F2 && (byte)UEdr["UEHandMade"] != 1)
				{
					MarkUnrecExpAsForbidden(UEdr);
					GoToNextUnrecExp(gvUnrecExp.FocusedRowHandle+1);
				}
			}
		}

		private void SetReserved(bool reserved, int FocusedRowHandle)
		{
			DataRow drUnrecExp = gvUnrecExp.GetDataRow(FocusedRowHandle);
			drUnrecExp["UEJunk"]=Convert.ToByte(reserved);
		}

		private void UnmarkUnrecExpAsNameForm(int FocusedRowHandle)
		{
			try
			{
				if ((GetMask(FocusedRowHandle, "UEStatus") & FormMask.NameForm) == FormMask.NameForm)
				{
					DataRow drUnrecExp = gvUnrecExp.GetDataRow(FocusedRowHandle);
					drUnrecExp["UEStatus"] = (int)((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) & (~FormMask.NameForm));
					drUnrecExp["UETmpProductId"] = 0;
				}
			}
			catch
			{
			}
		}

		private void UnmarkUnrecExpAsFirmForm(int FocusedRowHandle)
		{
			try
			{
				if ((GetMask(FocusedRowHandle, "UEStatus") & FormMask.FirmForm) == FormMask.FirmForm)
				{
					DataRow drUnrecExp = gvUnrecExp.GetDataRow(FocusedRowHandle);
					drUnrecExp["UEStatus"] = (int)((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) & (~FormMask.FirmForm));
					drUnrecExp["UETmpCodeFirmCr"]=0;
				}
			}
			catch
			{
			}
		}

		private void UnmarkUnrecExpAsForbidden(int FocusedRowHandle)
		{
			if ((GetMask(FocusedRowHandle, "UEStatus") & FormMask.MarkForb) == FormMask.MarkForb)
			{
				DataRow drUnrecExp = gvUnrecExp.GetDataRow(FocusedRowHandle);
				drUnrecExp["UEStatus"] = (int)((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) & (~FormMask.MarkForb));
			}
		}

		private void MainTimer_Tick(object sender, System.EventArgs e)
		{
			if (tcMain.SelectedTab == tpJobs && LockedPriceCode == -1)
			{
//				DateTime n = DateTime.Now;
//				try
//				{
//					Trace.Write(n);
//					Trace.WriteLine(" Start JobsGridFill");
					JobsGridFill();
//					Trace.Write(n);
//					Trace.WriteLine(" Stop  JobsGridFill");
//				}
//				catch(Exception ex)
//				{
//					//There is already an open DataReader associated with this Connection which must be closed first.
//
//					Trace.WriteLine(String.Format("Error on MainTimer : {0}", ex));
//					//MessageBox.Show(ex.ToString());
//				}
			}
		}

		private void MarkUnrecExpAsNameForm(DataRow drUnrecExp, bool MarkAsJUNK)
		{
			if (((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) &  FormMask.NameForm) != FormMask.NameForm)
			{
				//TODO: Здесь потребуется завести дополнительный столбец в таблицу нераспознанных выражений
				drUnrecExp["UEStatus"] = (int)((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) | FormMask.NameForm);
				drUnrecExp["UEJunk"] = Convert.ToByte(MarkAsJUNK);
				GridView bv = (GridView)CatalogGridControl.FocusedView;
				drUnrecExp["UETmpProductId"] = bv.GetDataRow(bv.FocusedRowHandle)["Id"];
			}
		}

		private void MarkUnrecExpAsFirmForm(DataRow drUnrecExp)
		{
			DataRow drCatalogFirmCr = gvFirmCr.GetDataRow(gvFirmCr.FocusedRowHandle);

			if (((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) &  FormMask.FirmForm) != FormMask.FirmForm)
			{
				drUnrecExp["UEStatus"] = (int)((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) | FormMask.FirmForm);
				drUnrecExp["UETmpCodeFirmCr"] = drCatalogFirmCr["CCode"];
			}
		}

		private void MarkUnrecExpAsForbidden(DataRow drUnrecExp)
		{
			if (((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) &  FormMask.MarkForb) != FormMask.MarkForb)
				drUnrecExp["UEStatus"] = (int)((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) | FormMask.MarkForb);
		}


		private string PrepareArg(string source)
		{
			string res = String.Empty;
			foreach(char c in source)
			{
				if (c == '*' || c == '%' || c == '[' || c == ']')
					res += String.Format("[{0}]", c);
				else
					res += Char.ToUpper(c).ToString();
			}
			return res;
		}

		private void CatalogGridControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			GridView FocusedView = (GridView)CatalogGridControl.FocusedView;
			//Снимаем фильтр при поиске
			if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || e.KeyCode == Keys.OemCloseBrackets ||
				e.KeyCode == Keys.OemOpenBrackets || e.KeyCode == Keys.OemSemicolon || e.KeyCode == Keys.OemQuotes ||
				e.KeyCode == Keys.Oemcomma || e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.OemQuestion ||
				(e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9))
			{
				FocusedView.ActiveFilter.Clear();
			}

			//Сбросили фильтр
			if (e.KeyCode == Keys.A && e.Control)
				FocusedView.ActiveFilter.Clear();

			if (e.KeyCode == Keys.Escape)
			{
				if (FocusedView.ParentView == null)
				{
					//Нажали Escape в корне (наименование), то идем к следующей нераспознанной позиции
					ClearCatalogGrid();
					GoToNextUnrecExp(gvUnrecExp.FocusedRowHandle + 1);
				}
				else
				{
					GridView Parent = (GridView)FocusedView.ParentView;
					//поднимаемся на уровень вверх
					Parent.CollapseMasterRow(Parent.FocusedRowHandle);
					Parent.ZoomView();
				}
			}

			if (String.IsNullOrEmpty(FocusedView.LevelName))
			{
				//Обработка первого уровня : Имени
				//Нажали Enter, значит идем на уровень вниз
				if (e.KeyCode == Keys.Enter)
					if (gvCatalog.FocusedRowHandle != GridControl.InvalidRowHandle)
					{
						gvCatalog.ExpandMasterRow(gvCatalog.FocusedRowHandle);
						GridView bv = (GridView)gvCatalog.GetDetailView(gvCatalog.FocusedRowHandle, 0);
						if (bv != null)
						{
							bv.ZoomView();
							bv.MoveFirst();
							CatalogGridControl.FocusedView = bv;
							bv.ActiveFilter.Clear();
							bv.ActiveFilter.Add(bv.Columns["Form"], new ColumnFilterInfo(GetFilterString(GetFullUnrecName(gvUnrecExp.FocusedRowHandle), "Form"), ""));
							if (bv.DataRowCount == 0)
								bv.ActiveFilter.Clear();
							else
								GotoCatalogPosition(bv, GetFullUnrecName(gvUnrecExp.FocusedRowHandle), "Form");

							DataRow dr = gvCatalog.GetDataRow(gvCatalog.FocusedRowHandle);
							colFForm.Caption = dr[colCName.FieldName].ToString();
						}
					}


				//Пометили позицию как запрещенную (нераспознанную)
				if (e.KeyCode == Keys.F2 && (gvUnrecExp.FocusedRowHandle != GridControl.InvalidRowHandle))
				{
					DataRow UEdr = gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);

					if ((byte)UEdr["UEHandMade"] != 1)
					{
						MarkUnrecExpAsForbidden(UEdr);
						GoToNextUnrecExp(gvUnrecExp.FocusedRowHandle + 1);
					}
				}
			}
			else
				if (FocusedView.LevelName == gvCatForm.LevelName)
				{
					if (e.KeyCode == Keys.Enter)
					{
						if (FocusedView.FocusedRowHandle != GridControl.InvalidRowHandle)
						{
							DataRow drCatalog = FocusedView.GetDataRow(FocusedView.FocusedRowHandle);
							dtProducts.Clear();
							ProductsFill(MyCmd, MyDA, (ulong)drCatalog[colCatalogID]);

							FocusedView.ExpandMasterRow(FocusedView.FocusedRowHandle);
							GridView bv = (GridView)FocusedView.GetDetailView(FocusedView.FocusedRowHandle, 0);
							if (bv != null)
							{
								bv.ZoomView();
								bv.MoveFirst();
								CatalogGridControl.FocusedView = bv;
								//Если нет других свойств, то просто сопоставляем с первым продуктом								
								if ((bv.DataRowCount == 1) && (bv.GetDataRow(0)["Properties"] is DBNull))
								{
									//Производим сопоставление
									DoSynonym(e.Shift);
									ChangeBigName(gvUnrecExp.FocusedRowHandle);
								}
								else
								{
									//Устанавливаем позицию на более подходящем продукте.
									GotoProductPosition(bv, GetFullUnrecName(gvUnrecExp.FocusedRowHandle), "Properties");
								}
								colProperties.Caption = colFForm.Caption + " - " + drCatalog[colFForm.FieldName].ToString();
							}
						}
					}
				}
				else
					if (FocusedView.LevelName == gvProducts.LevelName)
					{
						//Обработка третьего уровня : Продуктов со свойствами

						if (e.KeyCode == Keys.Enter)
							if (FocusedView.FocusedRowHandle != GridControl.InvalidRowHandle)
							{
								//Производим сопоставление
								DoSynonym(e.Shift);
								ChangeBigName(gvUnrecExp.FocusedRowHandle);
							}
					}
		}

		private void ClearCatalogGrid()
		{
			UnrecExpGridControl.Focus();
			grpBoxCatalog2.Text = "Каталог";
			CatalogGridControl.FocusedView = gvCatalog;
			gvCatalog.FocusedRowHandle = 0;
			CatalogGridControl.Enabled = false;
			gvFirmCr.FocusedRowHandle = 0;
			gcFirmCr.Enabled = false;
		}

		private void GoToNextUnrecExp(int FromFocusHandle)
		{
			for(int i = FromFocusHandle; i < gvUnrecExp.RowCount; i++)
			{
				if (i != GridControl.InvalidRowHandle)
					if (((GetMask(i, "UEStatus") & FormMask.NameForm) != FormMask.NameForm) || ((GetMask(i, "UEStatus") & FormMask.FirmForm) != FormMask.FirmForm))
					{
						gvUnrecExp.FocusedRowHandle = i;
						break;
					}
			}
			ClearCatalogGrid();
			MoveToCatalog();
		}

		private void DoSynonym(bool MarkAsJUNK)
		{
			int CurrentFocusHandle = gvUnrecExp.FocusedRowHandle;
			string TmpName = GetFullUnrecName(CurrentFocusHandle);
			for(int i = 0; i < gvUnrecExp.RowCount; i++)
			{
				DataRow dr = gvUnrecExp.GetDataRow(i);
				if (dr != null)
				{
					string drName = GetFullUnrecName(i);
					if (drName == TmpName)
					{
						MarkUnrecExpAsNameForm(dr, MarkAsJUNK);

						if (String.Empty == dr["UEFirmCr"].ToString())
						{
							dr["UEStatus"] = (int)((FormMask)Convert.ToByte(dr["UEStatus"]) | FormMask.NameForm);
							//TODO: Здесь надо корректно 
							dr["UETmpCodeFirmCr"] = 1;
						}
					}
				}
			}
			GoToNextUnrecExp(CurrentFocusHandle);
		}

		private void DoSynonymFirmCr()
		{
			int CurrentFocusHandle = gvUnrecExp.FocusedRowHandle;
			string TmpFirm = GetFirmCr(CurrentFocusHandle);
			for(int i = 0; i < gvUnrecExp.RowCount; i++)
			{
				DataRow dr = gvUnrecExp.GetDataRow(i);
				if (dr != null)
				{
					string drFirm = GetFirmCr(i);
					if (drFirm == TmpFirm)
					{
						MarkUnrecExpAsFirmForm(dr);
					}
				}
			}
			//			MessageBox.Show("Проведено сопоставление по производителю");
			GoToNextUnrecExp(CurrentFocusHandle);
		}

		private string GetFirmCr(int FocusedRowHandle)
		{
			if (FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvUnrecExp.GetDataRow(FocusedRowHandle);
				if (dr != null)
					return dr["UEFirmCr"].ToString();
				else
					return String.Empty;
			}
			else
				return String.Empty;
		}

		private void JobsGridControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
					//Начинае разбор нераспознанных выражений
				case Keys.Enter:
					if (gvJobs.FocusedRowHandle != GridControl.InvalidRowHandle)
						LockJob();
					break;

					//Обновляем таблицу заданий вручную
				case Keys.F5:
					JobsGridFill();
					break;
			}
		}

		private void gvJobs_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
		{
			if (e.RowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvJobs.GetDataRow(e.RowHandle);
				if (dr != null)
				{
					if (dr["JBlockBy"].ToString() != "")
					{
						e.Appearance.BackColor = btnJobsBlock.BackColor;
					}
					else
					{
						if ((System.Int64)dr["JPos"] == 0)
							return;

						if ((System.Int64)dr["JNamePos"] == 0)
							e.Appearance.BackColor = btnJobsNamePos.BackColor;

						double jnp = Convert.ToDouble(dr["JNamePos"]), jp = Convert.ToDouble(dr["JPos"]);

						if( ( ( jnp/(jp+1) ) * 100) < 50 )
							e.Appearance.BackColor = btnJobs50.BackColor;
					}
				}	
			}
		}

		//Блокируем задачу
		private void LockJob()
		{
			JobsGridFill();
			if (gvJobs.FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvJobs.GetDataRow(gvJobs.FocusedRowHandle);
				if (dr[colJBlockBy.FieldName].ToString() == String.Empty || dr[colJBlockBy.FieldName].ToString() == Environment.UserName)
				{
					LockedPriceCode = Convert.ToInt64(dr["JPriceCode"]);
                    PriceFMT = dr[JPriceFMT].ToString();
                    FileExt = dr[JExt].ToString();
                    if (dr[JParentSynonym] is DBNull)
						LockedSynonym = LockedPriceCode;
					else
						LockedSynonym = Convert.ToInt64(dr[JParentSynonym]);
					LockedInBlockedPrice(LockedPriceCode, Environment.UserName);
					grpBoxCatalog2.Text = "Каталог";

					tcMain.TabPages.Add(tpUnrecExp);
					tcMain.TabPages.Add(tpZero);
					tcMain.TabPages.Add(tpForb);

					this.Text += String.Format("   --  {0}", dr[colJName.FieldName].ToString());
					
					tcMain.TabPages.Remove(tpJobs);

					tcMain.SelectedTab = tpUnrecExp;

					UnrecExpGridFill(MyCmd, MyDA);

					dtUnrecExp.DefaultView.RowFilter = "UEAlready <> 5";
					if (dtUnrecExp.DefaultView.Count == 0)
					{
						dtUnrecExp.DefaultView.RowFilter = null;
						btnHideUnformFirmCr.Text = "Скрыть нераспознанные только по производителю";
					}
					else
						btnHideUnformFirmCr.Text = "Показать все";

					UnrecExpGridControl.Focus();
					gvUnrecExp.FocusedRowHandle = GridControl.InvalidRowHandle;
					gvUnrecExp.FocusedRowHandle = 0;
					GoToNextUnrecExp(0);
					sbpAll.Text = String.Format("Общее количество: {0}", dtUnrecExp.Rows.Count);
				}
			}
		}

		//Разблокируем задачу
		private void UnlockJob(DialogResult DRes)
		{  
			switch (DRes)
			{
				case DialogResult.Yes:
				{
					f = new frmProgress();

					Thread t = new Thread( new ThreadStart( ThreadMethod ) );
					t.Start();

					DialogResult dr = f.ShowDialog();
					f = null;

					if (dr == DialogResult.Cancel)
						t.Abort();
					
					goto case DialogResult.No;
				}
				case DialogResult.No:
				{
					tcMain.TabPages.Add(tpJobs);
					tcMain.TabPages.Remove(tpUnrecExp);
					tcMain.TabPages.Remove(tpZero);
					tcMain.TabPages.Remove(tpForb);
					tcMain.SelectedTab = tpJobs;
					JobsGridControl.Select();
					LocateJobs(LockedPriceCode, null);
					UnLockedInBlockedPrice(LockedPriceCode);
					LockedPriceCode = -1;
                    PriceFMT = String.Empty;
                    FileExt = String.Empty;
                    LockedSynonym = -1;
					gvUnrecExp.FocusedRowHandle = GridControl.InvalidRowHandle;
					dtUnrecExp.Clear();
					//Обновляем таблицу заданий
					JobsGridFill();
					sbpAll.Text = String.Empty;
					sbpCurrent.Text = String.Empty;
					this.Text = "Редактор нераспознанных выражений";
					return;
				}
				case DialogResult.Cancel:
				{
					return;
				}
			}
		}

		private void ThreadMethod()
		{
			try
			{
				ApplyChanges();
				f.Stop = true;
			}
			catch(ThreadAbortException)
			{
				//MessageBox.Show("Поймали исключение ThreadAbortException : {0}", e.ToString());
			}
			catch(Exception e)
			{
				if (f != null)
					f.Error = String.Format("Поймали исключение : {0}", e.ToString());
			}
		}

		delegate bool ShowRetransPriceDelegate();

		private bool ShowRetransPrice()
		{
			string str = String.Empty;
			str = String.Format(
@"Создано:
	запрещённых выражений - {0}
Синонимов:
	по наименованию - {1}
	по производителю - {2}
Отклонено скрытых синонимов: {3}
Отклонено дублирующихся синонимов: {4}

Перепровести прайс?", ForbiddenCount, SynonymCount, SynonymFirmCrCount, HideSynonymCount, DuplicateSynonymCount);
			return (MessageBox.Show(str, "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes);
		}

		private void ApplyChanges()
		{
			bool res = false;
			//Имеются ли родительские синонимы
			bool HasParentSynonym = LockedSynonym != LockedPriceCode;
			DateTime now = DateTime.Now;
			f.Status = "Подготовка таблиц...";

			//Список прайсов, которые нужно перепровести
			List<RetransedPrice> RetransedPriceList = new List<RetransedPrice>();

			//Если прайс-лист уже имеет родительский синоним, то ищем FileExtention родителя
			//и добавляем родителя первым в списке
			if (LockedSynonym != LockedPriceCode)
			{
				object ParentFileExt = MySqlHelper.ExecuteScalar(MyCn, @"
select 
  pf.FileExtention 
from 
  farm.formrules fr, 
  farm.pricefmts pf,
  usersettings.price_update_info pui 
where 
    fr.FirmCode = ?LockedSynonym 
and pf.Format = fr.PriceFmt
and pui.PriceCode = fr.FirmCode
and (pui.UnformCount > 0)", 
							new MySqlParameter("?LockedSynonym", LockedSynonym));
				//Первым в списке добавляем прайс-лист c родительскими синонимами
				if ((ParentFileExt != null) && !(ParentFileExt is DBNull) && (ParentFileExt is String))
					RetransedPriceList.Add(new RetransedPrice(LockedSynonym, (string)ParentFileExt));
			}
			else
				//Первым в списке добавляем сам прайс-лист
				RetransedPriceList.Add(new RetransedPrice(LockedPriceCode, FileExt));

			//Попытка найти всех потомков, которые используют родительские синонимы
			DataSet dsInerPrices = MySqlHelper.ExecuteDataset(MyCn, @"
select
  pd.PriceCode,
  pf.FileExtention
from
  farm.formrules fr,
  usersettings.pricesdata pd,
  usersettings.clientsdata cd,
  usersettings.pricescosts pc,
  usersettings.pricesdata parentpd,
  farm.pricefmts pf,
  usersettings.price_update_info pui
where
    fr.ParentSynonym = ?LockedSynonym
and pd.PriceCode = fr.FirmCode
and pf.Format = fr.PriceFmt
and pd.AgencyEnabled = 1
and cd.FirmCode = pd.FirmCode
and cd.FirmStatus = 1
and cd.BillingStatus = 1
and pc.PriceCode = pd.PriceCode
and parentpd.PriceCode = pc.ShowPriceCode
and ((pc.PriceCode = pc.ShowPriceCode) or (parentpd.CostType = 1))
and pui.PriceCode = pd.PriceCode
and (pui.UnformCount > 0)
",
				new MySqlParameter("?LockedSynonym", LockedSynonym));

			//Если в наборе данных будут записи, то добавляем их в список
			if (dsInerPrices.Tables[0].Rows.Count > 0)
			{
				HasParentSynonym = true;
				foreach(DataRow drInerPrice in dsInerPrices.Tables[0].Rows)
					RetransedPriceList.Add(
						new RetransedPrice(
							Convert.ToInt64(drInerPrice["PriceCode"]), 
							drInerPrice["FileExtention"].ToString()));
			}

			//Если в списке нет прайс-листа, с которым происходит работа, то мы его добавляем в список
			if (!RetransedPriceList.Exists(delegate(RetransedPrice value) { return (value.PriceCode == LockedPriceCode);}) )
				RetransedPriceList.Add(new RetransedPrice(LockedPriceCode, FileExt));

			SynonymCount = 0; 
			SynonymFirmCrCount = 0;
			ForbiddenCount = 0;
			HideSynonymCount = 0;
			DuplicateSynonymCount = 0;

			//Кол-во удаленных позиций - если оно равно кол-во нераспознанных позиций, то прайс автоматически проводится
			int DelCount = 0;
			
			f.Pr = 1;
			//Заполнение таблиц перед вставкой

			//Заполнили таблицу нераспознанных наименований для обновления
			MySqlDataAdapter daUnrecUpdate = new MySqlDataAdapter("select * from farm.UnrecExp where PriceCode = ?PriceCode", MyCn);
			MySqlCommandBuilder cbUnrecUpdate = new MySqlCommandBuilder(daUnrecUpdate);
			daUnrecUpdate.SelectCommand.Parameters.AddWithValue("?PriceCode", LockedPriceCode);
			DataTable dtUnrecUpdate = new DataTable();
			daUnrecUpdate.Fill(dtUnrecUpdate);
			dtUnrecUpdate.Constraints.Add("UnicNameCode", dtUnrecUpdate.Columns["RowID"], true);

			//Заполнили таблицу синонимов наименований
			MySqlDataAdapter daSynonym = new MySqlDataAdapter("select * from farm.Synonym where PriceCode = ?PriceCode limit 0", MyCn);
			//MySqlCommandBuilder cbSynonym = new MySqlCommandBuilder(daSynonym);
			daSynonym.SelectCommand.Parameters.AddWithValue("?PriceCode", LockedSynonym);
			DataTable dtSynonym = new DataTable();
			daSynonym.Fill(dtSynonym);
			dtSynonym.Constraints.Add("UnicNameCode", dtSynonym.Columns["Synonym"], false);
			daSynonym.InsertCommand = new MySqlCommand(
				@"
insert into farm.synonym (PriceCode, Synonym, Junk, ProductId) values (?PriceCode, ?Synonym, ?Junk, ?ProductId);
insert into logs.synonymlogs (LogTime, OperatorName, OperatorHost, Operation, SynonymCode, PriceCode, Synonym, Junk, ProductId)
  values (now(), ?OperatorName, ?OperatorHost, 0, last_insert_id(), ?PriceCode, ?Synonym, ?Junk, ?ProductId)", MyCn);
			daSynonym.InsertCommand.Parameters.AddWithValue("?OperatorName", Environment.UserName);
			daSynonym.InsertCommand.Parameters.AddWithValue("?OperatorHost", Environment.MachineName);
			daSynonym.InsertCommand.Parameters.Add("?PriceCode", MySqlDbType.UInt64, 0, "PriceCode");
			daSynonym.InsertCommand.Parameters.Add("?Synonym", MySqlDbType.VarString, 0, "Synonym");
			daSynonym.InsertCommand.Parameters.Add("?Junk", MySqlDbType.Byte, 0, "Junk");
			daSynonym.InsertCommand.Parameters.Add("?ProductId", MySqlDbType.UInt64, 0, "ProductId");
			
			f.Pr += 1;
			//Заполнили таблицу синонимов производителей
			MySqlDataAdapter daSynonymFirmCr = new MySqlDataAdapter("select * from farm.SynonymFirmCr where PriceCode = ?PriceCode limit 0", MyCn);
			//MySqlCommandBuilder cbSynonymFirmCr = new MySqlCommandBuilder(daSynonymFirmCr);
			daSynonymFirmCr.SelectCommand.Parameters.AddWithValue("?PriceCode", LockedSynonym);
			DataTable dtSynonymFirmCr = new DataTable();
			daSynonymFirmCr.Fill(dtSynonymFirmCr);
			dtSynonymFirmCr.Constraints.Add("UnicNameCode", new DataColumn[] {dtSynonymFirmCr.Columns["Synonym"]}, false);
			daSynonymFirmCr.InsertCommand = new MySqlCommand(
				@"
insert into farm.synonymFirmCr (PriceCode, CodeFirmCr, Synonym) values (?PriceCode, ?CodeFirmCr, ?Synonym);
insert into logs.synonymFirmCrLogs (LogTime, OperatorName, OperatorHost, Operation, SynonymFirmCrCode, PriceCode, CodeFirmCr, Synonym) 
  values (now(), ?OperatorName, ?OperatorHost, 0, last_insert_id(), ?PriceCode, ?CodeFirmCr, ?Synonym)", 
				MyCn);
			daSynonymFirmCr.InsertCommand.Parameters.AddWithValue("?OperatorName", Environment.UserName);
			daSynonymFirmCr.InsertCommand.Parameters.AddWithValue("?OperatorHost", Environment.MachineName);
			daSynonymFirmCr.InsertCommand.Parameters.Add("?PriceCode", MySqlDbType.UInt64, 0, "PriceCode");
			daSynonymFirmCr.InsertCommand.Parameters.Add("?Synonym", MySqlDbType.VarString, 0, "Synonym");
			daSynonymFirmCr.InsertCommand.Parameters.Add("?CodeFirmCr", MySqlDbType.UInt64, 0, "CodeFirmCr");

			f.Pr += 1;
			//Заполнили таблицу запрещённых выражений
			MySqlDataAdapter daForbidden = new MySqlDataAdapter("select * from farm.Forbidden limit 0", MyCn);
			//MySqlCommandBuilder cbForbidden = new MySqlCommandBuilder(daForbidden);
			DataTable dtForbidden = new DataTable();
			daForbidden.Fill(dtForbidden);
			dtForbidden.Constraints.Add("UnicNameCode", new DataColumn[] {dtForbidden.Columns["Forbidden"]}, false);
			daForbidden.InsertCommand = new MySqlCommand(
				@"
insert into farm.Forbidden (PriceCode, Forbidden) values (?PriceCode, ?Forbidden);
insert into logs.ForbiddenLogs (LogTime, OperatorName, OperatorHost, Operation, ForbiddenRowID, PriceCode, Forbidden) 
  values (now(), ?OperatorName, ?OperatorHost, 0, last_insert_id(), ?PriceCode, ?Forbidden);", 
				MyCn);
			daForbidden.InsertCommand.Parameters.AddWithValue("?OperatorName", Environment.UserName);
			daForbidden.InsertCommand.Parameters.AddWithValue("?OperatorHost", Environment.MachineName);
			daForbidden.InsertCommand.Parameters.Add("?PriceCode", MySqlDbType.UInt64, 0, "PriceCode");
			daForbidden.InsertCommand.Parameters.Add("?Forbidden", MySqlDbType.VarString, 0, "Forbidden");

			f.Pr = 10;

			for(int i = 0; i < gvUnrecExp.RowCount; i++)
			{
				if (i != GridControl.InvalidRowHandle)
				{
					DataRow dr = gvUnrecExp.GetDataRow(i);

					DelCount += UpDateUnrecExp(dtUnrecUpdate, dr);
					
					//Вставили новую запись в таблицу запрещённых выражений
					if (!MarkForbidden(i, "UEAlready") && MarkForbidden(i, "UEStatus"))
					{
						DataRow newDR = dtForbidden.NewRow();
								
						newDR["PriceCode"] = LockedSynonym;
						newDR["Forbidden"] = GetFullUnrecName(i);
						try
						{
							dtForbidden.Rows.Add(newDR);
							ForbiddenCount += 1;
						}
						catch(ConstraintException)
						{
						}
					}
					else
					{
						//Вставили новую запись в таблицу синонимов наименований
						if (NotNameForm(i, "UEAlready") && !NotNameForm(i, "UEStatus"))
						{
							DataRow newDR = dtSynonym.NewRow();

							newDR["PriceCode"] = LockedSynonym;
							newDR["Synonym"] = GetFullUnrecName(i);
							newDR["ProductId"] = dr[UETmpProductId];
							newDR["Junk"] = dr[UEJunk];
							try
							{
								dtSynonym.Rows.Add(newDR);
								SynonymCount += 1;
							}
							catch (ConstraintException)
							{
							}
						}

						//Вставили новую запись в таблицу синонимов производителей
						if (NotFirmForm(i, "UEAlready") && !NotFirmForm(i, "UEStatus"))
						{
							DataRow newDR = dtSynonymFirmCr.NewRow();

							newDR["PriceCode"] = LockedSynonym;
							newDR["CodeFirmCr"] = dr[UETmpCodeFirmCr];
							newDR["Synonym"] = GetFirmCr(i);
							try
							{
								dtSynonymFirmCr.Rows.Add(newDR);
								SynonymFirmCrCount +=1;
							}
							catch(ConstraintException)
							{
							}
						} 
					}
				}
			}

			f.Status = "Применение изменений в базу данных...";
			do
			{
				f.Pr = 30;
				MySqlTransaction tran = null;
				try
				{
					tran = MyCn.BeginTransaction(IsolationLevel.RepeatableRead);

					//Заполнили таблицу логов для синонимов наименований
					daSynonym.SelectCommand.Transaction = tran;
					DataTable dtSynonymCopy = dtSynonym.Copy();
					daSynonym.Update(dtSynonymCopy);

					f.Pr += 10;
                    
					//Заполнили таблицу логов для синонимов производителей
					daSynonymFirmCr.SelectCommand.Transaction = tran;
					DataTable dtSynonymFirmCrCopy = dtSynonymFirmCr.Copy();
					daSynonymFirmCr.Update(dtSynonymFirmCrCopy);

					MySqlHelper.ExecuteNonQuery(MyCn, @"
update usersettings.price_update_info
set
  LastSynonymsCreation = now()
where
  PriceCode = ?PriceCode",
								new MySqlParameter("?PriceCode", LockedSynonym)); 
					f.Pr += 10;
					
					//Заполнили таблицу логов для запрещённых выражений
					daForbidden.SelectCommand.Transaction = tran;
					DataTable dtForbiddenCopy = dtForbidden.Copy();
					daForbidden.Update(dtForbiddenCopy);

					f.Pr += 10;
                   
					//Обновление таблицы нераспознанных выражений
					daUnrecUpdate.SelectCommand.Transaction = tran;
					DataTable dtUnrecUpdateCopy = dtUnrecUpdate.Copy();
					daUnrecUpdate.Update(dtUnrecUpdateCopy);

					if (HasParentSynonym)
					{
						foreach (RetransedPrice rp in RetransedPriceList)
							MySqlHelper.ExecuteNonQuery(MyCn, @"
delete
from
  farm.UnrecExp
where
  PriceCode = ?DeletePriceCode
and not Exists(select * from farm.blockedprice bp where bp.PriceCode = ?DeletePriceCode and bp.BlockBy <> ?LockUserName)",
										new MySqlParameter("?DeletePriceCode", rp.PriceCode),
										new MySqlParameter("?LockUserName", Environment.UserName));
					}

					//DelCount = UpDateUnrecExp(tran);

					tran.Commit();
					res = true;

					f.Pr +=10;
				}
				catch(MySqlException ex)
				{
					try{ 
						tran.Rollback(); } 
					catch{}
					f.Error = String.Format("При обновлении синонимов произошла ошибка : {0}\r\n", ex);
					f.Pr = 50;
					Thread.Sleep(500);
				}
				finally
				{
					if (tran != null && !res)
						try{ 
							tran.Rollback(); } 
						catch{}
				}
			}
			while(!res);
			
			f.Pr = 80;

			f.Status = String.Empty;
			f.Error = String.Empty;

			bool S = DelCount == dtUnrecExp.Rows.Count;
			if (!S)
				S = (bool)f.Invoke( new ShowRetransPriceDelegate( ShowRetransPrice ) );

			if (res &&  S)
			{

#if DEBUG
				string rootpath = @"C:\Temp\";
#else
				string rootpath = @"\\fms\Prices\";
#endif

				f.Status = "Перепроведение пpайса...";
				f.Pr = 80;

				int CurrentPriceCode = 0;
				string CurrentFileName;
				do
				{
					CurrentFileName = RetransedPriceList[CurrentPriceCode].PriceCode.ToString() + RetransedPriceList[CurrentPriceCode].FileExt;
					try
					{
						if (File.Exists(rootpath + "Base\\" + CurrentFileName))
						{
							if (!File.Exists(rootpath + "Inbound0\\" + CurrentFileName))
							{
								File.Copy(rootpath + "Base\\" + CurrentFileName, rootpath + "Inbound0\\" + CurrentFileName);
								PricesRetrans(now, RetransedPriceList[CurrentPriceCode].PriceCode);
							}
						}
						RetransedPriceList.RemoveAt(CurrentPriceCode);
					}
					catch (Exception e)
					{
						if (f != null)
							f.Error = String.Format("При копировании файла {1} возникла ошибка : {0}\r\n", e, CurrentFileName);
						CurrentPriceCode++;
						Thread.Sleep(500);
					}
					if (CurrentPriceCode >= RetransedPriceList.Count)
						CurrentPriceCode = 0;
				}
				while(RetransedPriceList.Count > 0);

			}

			f.Pr = 100;
		}

		private void PricesRetrans(DateTime now, long RetransPriceCode)
		{
			MySqlCommand mcInsert = new MySqlCommand();
			mcInsert.Connection = MyCn;
			mcInsert.Parameters.Clear();
			mcInsert.Parameters.AddWithValue("?RetransPriceCode", RetransPriceCode);
			mcInsert.Parameters.AddWithValue("?UserName", Environment.UserName);
			mcInsert.Parameters.AddWithValue("?UserHost", Environment.MachineName);
			mcInsert.Parameters.AddWithValue("?Now", now);

			mcInsert.CommandText = 
					@"insert into logs.pricesretrans 
						(LogTime, 
						OperatorName,
						OperatorHost,
						PriceCode) 
					values 
						(?Now,
						?UserName,
						?UserHost,
						?RetransPriceCode)";
	
			mcInsert.ExecuteNonQuery();
		}

		private int UpDateUnrecExp(DataTable dtUnrecExpUpdate, DataRow drUpdated)
		{
			int DelCount = 0;

			int FULLFORM = (int)(FormMask.NameForm | FormMask.FirmForm | FormMask.CurrForm);

			//Производим проверку того, что синоним может быть сопоставлен со скрытым каталожным наименованием
			bool HidedSynonym = Convert.ToBoolean(
				MySqlHelper.ExecuteScalar(MyCn,
				String.Format(@"
select
  (products.Hidden or catalog.Hidden) as Hidden
from
  catalogs.catalog,
  catalogs.products
where
    products.Id = {0}
and catalog.Id = products.CatalogId", drUpdated[UETmpProductId]
					)
				)
			);
			if (HidedSynonym)
			{
				//Если в процессе распознования каталожное наименование скрыли, то сбрасываем распознавание
				drUpdated["UETmpProductId"] = 0;
				drUpdated["UEStatus"] = (int)((FormMask)Convert.ToByte(drUpdated["UEStatus"]) & (~FormMask.NameForm));
				HideSynonymCount++;
			}

			//Производим проверку того, что синоним может быть уже вставлен в таблицу синонимов
			object SynonymExists = MySqlHelper.ExecuteScalar(MyCn, 
				"select ProductId from farm.synonym where synonym = ?Synonym and PriceCode=" + LockedSynonym.ToString(), 
				new MySqlParameter("?Synonym", String.Format("{0} {1} {2}", drUpdated["UEName1"], drUpdated["UEName2"], drUpdated["UEName3"])));
			if ((SynonymExists != null))
			{
				//Если в процессе распознования синоним уже кто-то добавил, то сбрасываем распознавание
				drUpdated["UETmpProductId"] = 0;
				drUpdated["UEStatus"] = (int)((FormMask)Convert.ToByte(drUpdated["UEStatus"]) & (~FormMask.NameForm));
				DuplicateSynonymCount++;
			}

			DataRow drNew = dtUnrecExpUpdate.Rows.Find( Convert.ToUInt32( drUpdated["UERowID"] ) );

			if (drNew != null)
			{

				if ((int)drUpdated["UEStatus"] == FULLFORM)
				{
					drNew.Delete();
					DelCount++;
				}
				else
				{
					drNew["Status"] = drUpdated["UEStatus"];
					drNew["TmpProductId"] = drUpdated["UETmpProductId"];
					drNew["TmpCodeFirmCr"] = drUpdated["UETmpCodeFirmCr"];
					drNew["TmpCurrency"] = drUpdated["UETmpCurrency"];
					drNew["RowID"] = drUpdated["UERowID"];
					if ((byte)drUpdated["UEHandMade"] == 0)
					{
						int r = (int)drUpdated["UEStatus"] ^ (int)drUpdated["UEAlready"];
						if ( (r > 0 && (r & (int)FormMask.MarkForb) == 0))
						{
							drNew["HandMade"] = 1;
						}
						else
						{
							drNew["HandMade"] = 0;
						}
					}
					else
						drNew["HandMade"] = 1;
				}

			}

			return DelCount;
		}

		private void frmUEEMain_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
					//Если нажали F12 на вкладке нераспознанные выражения, то завершаем распознование
				case Keys.F12:
					if (tcMain.SelectedTab == tpUnrecExp)
					{
						DialogResult DRes;
						DRes = MessageBox.Show("Сохранить результаты?" , "Вопрос", MessageBoxButtons.YesNoCancel);
						UnlockJob(DRes);
					}
					break;

				case Keys.F11:
					UpdateCatalog();
					break;
			}
		}

		private void LocateJobs(long JCode, List<long> selectedPrices)
		{
			if ((JCode != -1) || (selectedPrices != null))
			{
				int FocusedRowHandle = GridControl.InvalidRowHandle;

				for (int i = 0; i < gvJobs.RowCount; i++)
				{
					DataRow dr = gvJobs.GetDataRow(i);
					if ((selectedPrices != null) && (selectedPrices.Contains((long)dr[JPriceCode.ColumnName])))
						gvJobs.SelectRow(i);
					if ((JCode != -1) && (JCode == (long)dr[JPriceCode.ColumnName]))
					{
						FocusedRowHandle = i;
						if (selectedPrices == null)
							break;
					}
				}

				if (FocusedRowHandle != GridControl.InvalidRowHandle)
					gvJobs.FocusedRowHandle = FocusedRowHandle;
			}
		}

		private void LockedInBlockedPrice(long LockPriceCode, string BlockBy)
		{
			MySqlCommand mcInsert = new MySqlCommand("select * from blockedprice where PriceCode = ?LockPriceCode", MyCn);
			mcInsert.Parameters.Clear();
			mcInsert.Parameters.AddWithValue("?LockPriceCode", LockedPriceCode);
			MySqlDataReader drInsert = mcInsert.ExecuteReader();
			bool NotExist = !drInsert.Read();
			drInsert.Close();
			drInsert = null;
			if (NotExist)
			{
				mcInsert.CommandText = @"insert into blockedprice (PriceCode, BlockBy) values (?LockPriceCode, ?BlockBy)";
				mcInsert.Parameters.AddWithValue("?BlockBy", BlockBy);
				mcInsert.ExecuteNonQuery();
			}
		}

		private void UnLockedInBlockedPrice(long LockPriceCode)
		{
			MySqlCommand mcInsert = new MySqlCommand("delete from blockedprice where PriceCode = ?LockPriceCode", MyCn);
			mcInsert.Parameters.Clear();
			mcInsert.Parameters.AddWithValue("?LockPriceCode", LockedPriceCode);
			mcInsert.ExecuteNonQuery();
		}

		private void ChangeBigName(int FocusedRowHandle)
		{
			if (FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				if (NotNameForm(FocusedRowHandle, "UEStatus"))
					BigNameLabel2.Text = GetFullUnrecName(FocusedRowHandle);
				else
					if (NotFirmForm(FocusedRowHandle, "UEStatus"))
					BigNameLabel2.Text = GetFirmCr(FocusedRowHandle);
				else
					BigNameLabel2.Text = GetFullUnrecName(FocusedRowHandle);

				sbpCurrent.Text = String.Format("Текущая позиция: {0}", FocusedRowHandle+1);
			}
			else
				sbpCurrent.Text = String.Empty;
		}

		private void gvUnrecExp_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
		{
			ChangeBigName(e.FocusedRowHandle);
		}

		private string GetFullUnrecName(int FocusedRowHandle)
		{
			if (FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvUnrecExp.GetDataRow(FocusedRowHandle);
				if (dr != null)
					return String.Format("{0} {1} {2}", dr["UEName1"], dr["UEName2"], dr["UEName3"]);
				else
					return String.Empty;
			}
			else
				return String.Empty;
		}

		private void gvUnrecExp_CustomColumnSort(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnSortEventArgs e)
		{
			if (e.Column.FieldName == "UEAlready")
			{
				int v1 = (int)e.Value1;
				int v2 = (int)e.Value2;
				e.Handled = true;
				if (v1 == v2)
					e.Result = 0;
				else
					if (v1 == 6)
				{
					if (v2 == 0)
						e.Result = 1;
					else
						e.Result = -1;
				}
				else
					if (v2 == 6)
				{
					if (v1 == 0)
						e.Result = -1;
					else
						e.Result = 1;
				}
				else
					e.Result = v1-v2;
			}
		}

		private void gvJobs_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
		{
		    if (e.Column == colJWholeSale)
		    {
			    if (e.Value.ToString() == "0")
				    e.DisplayText = "Опт";
			    else
				    e.DisplayText = "Розница";
		    }
            if ((e.Column == colJNeedRetrans)||(e.Column == colJRetranced))
            {
                if (e.Value.ToString() == "1")
                    e.DisplayText = "Да";
                else
                    e.DisplayText = "Нет";
            }
		}

		private void UnrecExpGridControl_Click(object sender, System.EventArgs e)
		{
			if (gvUnrecExp.FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				CatalogGridControl.Enabled = false;
				ClearCatalogGrid();
			}
		}

		private void gvJobs_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
		{
			if (e.RowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvJobs.GetDataRow(e.RowHandle);
				if ((dr["JBlockBy"].ToString() != ""))
				{
					Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.Gray, Color.Gray, 90);
					Rectangle  r = e.Bounds;
					r.Inflate(-1, -1);
					//e.Graphics.FillRectangle(brush, r);

					int x = r.X + (r.Width - e.Info.Images.ImageSize.Width) / 2;
					int y = r.Y + (r.Height - e.Info.Images.ImageSize.Height) / 2;
					e.Graphics.DrawImageUnscaled(imageList1.Images[0], r.X, r.Y);
					ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
					e.Handled = true;
				}
			}		
		}

		private string ConvertName(string tmp)
		{
			string res = String.Empty;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i=0; i<tmp.Length; i++)
			{
				if (Char.IsLetterOrDigit(tmp[i]) )
				{
					sb.Append(tmp[i]);
				}
				else
				{
					sb.Append("%");
					sb.Append( Convert.ToInt32(tmp[i]).ToString("X2") );
				}
			}
			return sb.ToString();
		}

		private void miSendAboutNames_Click(object sender, System.EventArgs e)
		{	
			DataRow[] drs = dtJobs.Select("JPriceCode = " + LockedPriceCode.ToString());

			if (drs.Length > 0)
			{
				DataRow dr = drs[0];

				ArrayList NameArray = new ArrayList();

				foreach(DataRow UEdr in dtUnrecExp.Rows)
				{
					if ( ((FormMask)Convert.ToByte(UEdr["UEStatus"]) & FormMask.MarkForb) == FormMask.MarkForb )
					{
	
						string tmp = (UEdr["UEName1"].ToString() + " " + UEdr["UEFirmCr"].ToString()).Trim();
						if (!NameArray.Contains(tmp))
						{
							//ConvertName(tmp);
							NameArray.Add( tmp );
						}
					}
				}

				string UnrecName = String.Join("\r\n", (string[])NameArray.ToArray(typeof(string)));

				Clipboard.SetDataObject(UnrecName);

				string subject = String.Format(UEEditor.Properties.Settings.Default.AboutNamesSubject, dr["FirmShortName"]);

				string body = "";
                body = UEEditor.Properties.Settings.Default.AboutNamesBody;

				body = String.Format(body, dr["FirmShortName"]);

				System.Diagnostics.Process.Start(String.Format("mailto:{0}?cc={1}&Subject={2}&Body={3}", GetContactText((long)dr[JFirmCode.ColumnName], 2, 0), "pharm@analit.net", subject, body));
			}
		}

		private void miSendAboutFirmCr_Click(object sender, System.EventArgs e)
		{
			DataRow[] drs = dtJobs.Select("JPriceCode = " + LockedPriceCode.ToString());

			if (drs.Length > 0)
			{
				DataRow dr = drs[0];

				ArrayList FirmCrArray = new ArrayList();

				foreach(DataRow UEdr in dtUnrecExp.Rows)
				{
					if ( ((FormMask)Convert.ToByte(UEdr["UEStatus"]) & FormMask.FirmForm) != FormMask.FirmForm )
					{
						string tmp = UEdr["UEFirmCr"].ToString().Trim();
						if (!FirmCrArray.Contains(tmp))
						{
							FirmCrArray.Add(tmp);
						}
					}
				}

				string UnrecFirmCr = String.Join("\r\n", (string[])FirmCrArray.ToArray(typeof(string)));

				Clipboard.SetDataObject(UnrecFirmCr);

				string subject = String.Format(UEEditor.Properties.Settings.Default.AboutFirmSubject, dr["FirmShortName"]);

				string body = "";
                body = UEEditor.Properties.Settings.Default.AboutFirmBody;

				body = String.Format(body, dr["FirmShortName"]);

				System.Diagnostics.Process.Start(String.Format("mailto:{0}?cc={1}&Subject={2}&Body={3}", GetContactText((long)dr[JFirmCode.ColumnName], 2, 0), "pharm@analit.net", subject, body));
			}
		}

		/// <summary>
		/// Получить текст контактов из базы
		/// </summary>
		/// <param name="FirmCode">Код поставщика</param>
		/// <param name="ContactGroupType">Тип контактной группы: 0 - General, 1 - ClientManager, 2 - OrderManager, 3 - Accountant</param>
		/// <param name="ContactType">Тип контакта: 0 - Email, 1 - Phone</param>
		/// <returns>Текст контактов, разделенный ";"</returns>
		private string GetContactText(long FirmCode, byte ContactGroupType, byte ContactType)
		{
			DataSet dsContacts = MySqlHelper.ExecuteDataset(MyCn, @"
select distinct c.contactText
from usersettings.clientsdata cd
  join contacts.contact_groups cg on cd.ContactGroupOwnerId = cg.ContactGroupOwnerId
    join contacts.contacts c on cg.Id = c.ContactOwnerId
where
    firmcode = ?FirmCode
and cg.Type = ?ContactGroupType
and c.Type = ?ContactType

union

select distinct c.contactText
from usersettings.clientsdata cd
  join contacts.contact_groups cg on cd.ContactGroupOwnerId = cg.ContactGroupOwnerId
    join contacts.persons p on cg.id = p.ContactGroupId
      join contacts.contacts c on p.Id = c.ContactOwnerId
where
    firmcode = ?FirmCode
and cg.Type = ?ContactGroupType
and c.Type = ?ContactType;",
				new MySqlParameter("?FirmCode", FirmCode),
				new MySqlParameter("?ContactGroupType", ContactGroupType),
				new MySqlParameter("?ContactType", ContactType));
			List<string> contacts = new List<string>();
			foreach (DataRow drContact in dsContacts.Tables[0].Rows)
			{
				if (!contacts.Contains(drContact["contactText"].ToString()))
					contacts.Add(drContact["contactText"].ToString());
			}

			return String.Join(";", contacts.ToArray());
		}

		private void gvUnrecExp_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
		{
			if (e.RowHandle != GridControl.InvalidRowHandle)
			{
				if (e.Column == colUEColumn1)
				{
					if (((GetMask(e.RowHandle, "UEStatus") & FormMask.MarkForb) == FormMask.MarkForb))
					{
						Rectangle  r = e.Bounds;
						r.Inflate(-1, -1);
						System.Drawing.Brush br = new System.Drawing.SolidBrush(System.Drawing.SystemColors.Control);
						e.Graphics.FillRectangle(br, r);
						e.Graphics.DrawImageUnscaled(imageList2.Images[3], r.X, r.Y);
						ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.Adjust);
						e.Handled = true;
					}
					else
					if (((GetMask(e.RowHandle, "UEStatus") & FormMask.NameForm) == FormMask.NameForm))
					{
						Rectangle  r = e.Bounds;
						r.Inflate(-1, -1);
						System.Drawing.Brush br = new System.Drawing.SolidBrush(System.Drawing.SystemColors.Control);
						e.Graphics.FillRectangle(br, r);
						e.Graphics.DrawImageUnscaled(imageList2.Images[0], r.X, r.Y);
						ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.Adjust);
						e.Handled = true;
					}
				}

				if (e.Column == colUEColumn2)
				{
					if (((GetMask(e.RowHandle, "UEStatus") & FormMask.FirmForm) == FormMask.FirmForm) && ((GetMask(e.RowHandle, "UEStatus") & FormMask.MarkForb) != FormMask.MarkForb))
					{
						Rectangle  r = e.Bounds;
						r.Inflate(-1, -1);
						System.Drawing.Brush br = new System.Drawing.SolidBrush(System.Drawing.SystemColors.Control);
						e.Graphics.FillRectangle(br, r);
						e.Graphics.DrawImageUnscaled(imageList2.Images[1], r.X, r.Y);
						ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.Adjust);
						e.Handled = true;
					}
				}

				if (e.Column == colUEColumn3)
				{
					if (((GetMask(e.RowHandle, "UEStatus") & FormMask.CurrForm) == FormMask.CurrForm) && ((GetMask(e.RowHandle, "UEStatus") & FormMask.MarkForb) != FormMask.MarkForb))
					{
						Rectangle  r = e.Bounds;
						r.Inflate(-1, -1);
						System.Drawing.Brush br = new System.Drawing.SolidBrush(System.Drawing.SystemColors.Control);
						e.Graphics.FillRectangle(br, r);
						e.Graphics.DrawImageUnscaled(imageList2.Images[2], r.X, r.Y);
						ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.Adjust);
						e.Handled = true;
					}
				}
			}		
		}

		private void gvUnrecExp_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
		{
			if (e.RowHandle != GridControl.InvalidRowHandle)
			{
				int i = e.RowHandle;

				DataRow UEdr = gvUnrecExp.GetDataRow(i);

				if (UEdr != null)
				{
					if (e.Column.VisibleIndex ==0)
						e.Appearance.BackColor = Color.White;
					else
						if (e.Column.VisibleIndex ==1)
						e.Appearance.BackColor = Color.White;

					else
						if (e.Column.VisibleIndex ==2)
						e.Appearance.BackColor = Color.White;
					else
					{
						if (7 == (int)UEdr["UEStatus"])
							e.Appearance.BackColor = Color.Lime;
						else
							if (((GetMask(i, "UEStatus") & FormMask.MarkForb) == FormMask.MarkForb))
							e.Appearance.BackColor = SystemColors.GrayText;
						else
							if (((GetMask(i, "UEStatus") & FormMask.NameForm) == FormMask.NameForm))
							e.Appearance.BackColor = Color.PaleGreen;
					}
				}
			}
		}

		private void lbColorChange(object sender, System.EventArgs e)
		{
			cdLegend.Color = ((Button)sender).BackColor;
			if (cdLegend.ShowDialog((Button)sender) == DialogResult.OK)
			{
				((Button)sender).BackColor = cdLegend.Color;
				gvJobs.RefreshData();
			}
		}

		private void LoadColor(Button lColor, int defaultArgb)
		{
			using(RegistryKey k = Registry.CurrentUser.OpenSubKey(BaseRegKey, true))
			{
				int argbColor;
				if (k != null)
					argbColor = (int)k.GetValue(lColor.Name, defaultArgb);
				else
					argbColor = defaultArgb;
				lColor.BackColor = Color.FromArgb(argbColor);
			}
		}
				
		private void SaveColor(Button lColor)
		{
			using(RegistryKey k = Registry.CurrentUser.CreateSubKey(BaseRegKey))
			{
				if (k != null)
					k.SetValue(lColor.Name, lColor.BackColor.ToArgb());
			}
		}

		private void gvUnrecExp_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
		{
			if (e.Column == colUEJunk)
				e.DisplayText = ((byte)e.Value == 1) ? "Да" : "Нет";
		}

		private void gvProducts_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
		{
			if ((e.Column.Name == colProperties.Name) && (e.Value is DBNull))
				e.DisplayText = "[не установленны]";
		}

		private void gcFirmCr_KeyDown(object sender, KeyEventArgs e)
		{
			//Здесь будет обработка производителя
			//Снимаем фильтр при поиске
			if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || e.KeyCode == Keys.OemCloseBrackets ||
				e.KeyCode == Keys.OemOpenBrackets || e.KeyCode == Keys.OemSemicolon || e.KeyCode == Keys.OemQuotes ||
				e.KeyCode == Keys.Oemcomma || e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.OemQuestion ||
				(e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9))
			{
				gvFirmCr.ActiveFilter.Clear();
			}

			if (e.KeyCode == Keys.Enter)
				if (gvFirmCr.FocusedRowHandle != GridControl.InvalidRowHandle)
				{
					DataRow drUnrecExp = gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);
					if (((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) & FormMask.FirmForm) != FormMask.FirmForm)
					{
						DoSynonymFirmCr();
						ChangeBigName(gvUnrecExp.FocusedRowHandle);
					}
				}

			if (e.KeyCode == Keys.Escape)
			{
				ClearCatalogGrid();
				GoToNextUnrecExp(gvUnrecExp.FocusedRowHandle + 1);
			}

			if (e.KeyCode == Keys.A && e.Control)
				gvFirmCr.ActiveFilter.Clear();

			if (e.KeyCode == Keys.F2 && (gvUnrecExp.FocusedRowHandle != GridControl.InvalidRowHandle))
			{
				DataRow UEdr = gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);

				if ((byte)UEdr["UEHandMade"] != 1)
				{
					MarkUnrecExpAsForbidden(UEdr);
					GoToNextUnrecExp(gvUnrecExp.FocusedRowHandle + 1);
				}
			}
		}

		private void gvCatForm_RowStyle(object sender, RowStyleEventArgs e)
		{
			if ((e.RowHandle != GridControl.InvalidRowHandle) && (e.RowHandle != -1))
			{ 
				DataRow dr = ((GridView)sender).GetDataRow(e.RowHandle);

				if ((dr != null) && (Convert.ToInt64(dr[colCatalogProductsCount.ColumnName]) > 1))
					e.Appearance.BackColor = Color.LightGreen;
			}

		}

		private void gvCatForm_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
		{
			if ((e.Info.IsRowIndicator) && (e.RowHandle != GridControl.InvalidRowHandle) && (e.RowHandle != -1))
			{
				DataRow dr = ((GridView)sender).GetDataRow(e.RowHandle);

				if ((dr != null) && (Convert.ToInt64(dr[colCatalogProductsCount.ColumnName]) > 1))
					e.Info.BackAppearance.BackColor = Color.LightGreen;
			}
		}

		private void gvCatalog_CalcRowHeight(object sender, RowHeightEventArgs e)
		{
			if (e.RowHandle >= 0)
				e.RowHeight += 4;
		}

		private void btnHideUnformFirmCr_Click(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(dtUnrecExp.DefaultView.RowFilter))
			{
				dtUnrecExp.DefaultView.RowFilter = "UEAlready <> 5";
				btnHideUnformFirmCr.Text = "Показать все";
			}
			else
			{
				dtUnrecExp.DefaultView.RowFilter = null;
				btnHideUnformFirmCr.Text = "Скрыть нераспознанные только по производителю";
			}

			UnrecExpGridControl.Focus();
		}

	}

	public class RetransedPrice
	{
		public long PriceCode;
		public string FileExt;

		public RetransedPrice(long APriceCode, string AFileExt)
		{
			this.PriceCode = APriceCode;
			this.FileExt = AFileExt;
		}
	}

	internal class UEEditorExceptionHandler
	{

		// Handles the exception event.
		public void OnThreadException(object sender, System.Threading.ThreadExceptionEventArgs t)
		{
			try
			{
				System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
					"service@analit.net",
					"service@analit.net",
					"Необработанная ошибка в UEEditor",
					String.Format("Sender = {0}\r\nException = = {1}", sender, t.Exception));
				System.Net.Mail.SmtpClient sm = new System.Net.Mail.SmtpClient("box.analit.net");
				sm.Send(m);
			}
			catch
			{ }
			MessageBox.Show("В приложении возникла необработанная ошибка.\r\nИнформация об ошибке была отправлена разработчику.");
		}

	}
}
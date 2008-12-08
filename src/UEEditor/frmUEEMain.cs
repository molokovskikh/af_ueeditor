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
using Inforoom.Logging;



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

		//����� ���������� ���������� ��������
		private DateTime catalogUpdate;
        
		public string PriceFMT = String.Empty;
        public string FileExt = String.Empty;
		public long LockedPriceCode = -1;
		public long LockedPriceItemId = -1;
		public long LockedSynonym = -1;
		public frmProgress f = null;
		public int SynonymCount = 0;
		public int HideSynonymCount = 0;
		public int HideSynonymFirmCrCount = 0;
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

			MyCn.ConnectionString = "server=sql.analit.net; user id=AppUEEditor; password=samepass; database=farm;convert zero datetime=true;";
#if DEBUG
			MyCn.ConnectionString = "server=testsql.analit.net; user id=system; password=newpass; database=farm;convert zero datetime=true;";
#endif
			MyCn.Open();
			MyDA = new MySqlDataAdapter(MyCmd);
			MyCmd.Connection = MyCn;

			tcMain.TabPages.Remove(tpUnrecExp);
			tcMain.TabPages.Remove(tpZero);
			tcMain.TabPages.Remove(tpForb);

			//������� ����������� ��� ������� �������
			DAJobsCreate();

			//��������� ������� �������
			JobsGridFill();

			//�������� ���������� �������
			CatalogFirmCrGridFill(MyCmd, MyDA);

			CatalogNameGridFill(MyCmd, MyDA);

			FormGridFill(MyCmd, MyDA);

			catalogUpdate = DateTime.Now;

			//
			JobsGridControl.Select();
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(UEEditorExceptionHandler.OnThreadException);

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
				DRes = MessageBox.Show("�� ���������� � ������ �������������� �����-�����. ��������� ���������?", "������", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
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
SELECT
        PD.FirmCode as JFirmCode,
        cd.ShortName as FirmShortName,
        pim.Id as JPriceItemId,
        PD.PriceCode As JPriceCode,
        concat(CD.ShortName, ' (', if(pd.CostType = 1, concat(pd.PriceName, ' [�������] ', pc.CostName), pd.PriceName), ')') as JName,
        regions.region                                                                                                       As JRegion,
        pim.PriceDate as JPriceDate,
        statunrecexp.Pos                                                                        AS JPos,
        statunrecexp.NamePos                                                                    AS JNamePos,
        pim.LastFormalization                                                                   AS JJobDate,
        CD.FirmSegment                                                                          As JWholeSale,
        bp.BlockBy                                                                              As JBlockBy,
        PD.ParentSynonym                                                                        as JParentSynonym,
        pfmt.Format                                                                             As JPriceFMT,
        pfmt.FileExtention                                                                      as JExt,
        pim.LastFormalization                                                                   AS JDateLastForm,
        if((synonympim.LastSynonymsCreation is not null) and (pim.LastFormalization < synonympim.LastSynonymsCreation), 1, 0) AS JNeedRetrans,
        if(pim.LastFormalization < pim.LastRetrans, 1, 0)                                            AS JRetranced,
        if(pd.ParentSynonym is null, '', concat(synonymcd.ShortName, ' (', synonympd.PriceName, ')')) AS JParentName
FROM
  (
   (select
      unrecexp.PriceItemId,
      count(unrecexp.RowID) as Pos,
      COUNT(IF(unrecexp.TmpProductId is null, 1, null)) as NamePos
    from
      farm.unrecexp
    group by unrecexp.PriceItemId) statunrecexp,
   usersettings.priceitems pim,
   usersettings.pricescosts pc,
   usersettings.pricesdata AS PD,
   usersettings.ClientsData AS CD,
   farm.regions,
   farm.FormRules,
   farm.PriceFMTs as pfmt,
   usersettings.pricesdata synonympd,
   usersettings.pricescosts synonympc,
   usersettings.priceitems synonympim,
   usersettings.clientsdata synonymcd
  )
  LEFT JOIN farm.blockedprice bp ON bp.PriceItemId = pim.Id
WHERE
    pim.Id = statunrecexp.PriceItemId
and pc.PriceItemId = pim.Id
and pc.PriceCode = pd.PriceCode
and ((pd.CostType = 1) or (pc.BaseCost = 1))
AND pd.agencyenabled   =1
and pd.FirmCode = cd.FirmCode
AND regions.regioncode =CD.regioncode
and FormRules.id = pim.FormRuleId
and pfmt.Id = FormRules.PriceFormatId
and synonympd.PriceCode = ifnull(pd.ParentSynonym, PD.pricecode)
and synonympc.PriceCode = synonympd.PriceCode
and synonympc.BaseCost = 1
and synonympim.Id = synonympc.PriceItemId
and synonymcd.FirmCode = synonympd.FirmCode", 
				MyCn);
		}

		private void JobsGridFill()
		{
			long CurrPriceItemId = -1;
			List<long> selectedPrices = new List<long>();
			if (gvJobs.FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow drJ = gvJobs.GetDataRow(gvJobs.FocusedRowHandle);
				if (drJ != null)
					CurrPriceItemId = Convert.ToInt64(drJ[JPriceItemId.ColumnName]);
			}

			int[] selected = gvJobs.GetSelectedRows();
			if (selected.Length > 0)
			{
				//������� �����-����� �� ����, �.�. ����� ��������� ���������� �������
				foreach (int rowHandle in selected)
					if (rowHandle != GridControl.InvalidRowHandle)
						selectedPrices.Add((long)gvJobs.GetDataRow(rowHandle)[JPriceItemId.ColumnName]);
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

			LocateJobs(CurrPriceItemId, (selectedPrices.Count <= 1) ? null : selectedPrices);
			statusBar1.Panels[0].Text = "������� � �������: " + dtJobs.Rows.Count;
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
				  WHERE PriceItemId= ?LockedPriceItemId ORDER BY Name1";

			MyCmd.Parameters.Clear();
			MyCmd.Parameters.AddWithValue("?LockedPriceItemId", LockedPriceItemId);
			
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
                and Hidden = 0 
				Order By FirmCr";

			MyDA.Fill(dtCatalogFirmCr);

			//��������� � ������ ������� ������������ ������, ������������ ������� "������������� �� ��������"
			DataRow drUnknown = dtCatalogFirmCr.NewRow();
			drUnknown["CCode"] = 0;
			drUnknown["CName"] = "������������� �� ��������";
			dtCatalogFirmCr.Rows.InsertAt(drUnknown, 0);
		}

		private void FormGridFill(MySqlCommand MyCmd, MySqlDataAdapter MyDA)
		{
			MyCmd.CommandText =
				@"
select
  Catalog.*,
  CatalogForms.Form,
  count(pp.ProductId) as productscount
from
  catalogs.Catalog,
  catalogs.CatalogForms,
  catalogs.products
  left join catalogs.productproperties pp on pp.ProductId = products.id
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
  GROUP_CONCAT(PropertyValues.Value
    order by Properties.PropertyName, PropertyValues.Value
    SEPARATOR ', '
  ) as Properties
FROM
(
catalogs.Products,
catalogs.Catalog
)
left join catalogs.ProductProperties on ProductProperties.ProductId = Products.Id
left join catalogs.PropertyValues on PropertyValues.Id = ProductProperties.PropertyValueId
left join catalogs.Properties on Properties.Id = PropertyValues.PropertyId
where
    Catalog.Id = Products.CatalogID
and Catalog.Id = ?CatalogId
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
  cast(GROUP_CONCAT(PropertyValues.Value
    order by Properties.PropertyName, PropertyValues.Value
    SEPARATOR ', '
  ) as char) as Properties
FROM
(
catalogs.Products,
catalogs.Catalog
)
left join catalogs.ProductProperties on ProductProperties.ProductId = Products.Id
left join catalogs.PropertyValues on PropertyValues.Id = ProductProperties.PropertyValueId
left join catalogs.Properties on Properties.Id = PropertyValues.PropertyId
where
    Catalog.Id = Products.CatalogID
and Products.Id = ?ProductId 
and Products.Hidden = 0
group by Products.Id
order by Properties
";

			MyDA.Fill(dtProducts);
		}

		private void CheckCatalog()
		{
			DateTime CatalogUpdateTime = Convert.ToDateTime(MySqlHelper.ExecuteScalar(MyCn, "select max(UpdateTime) from catalogs.catalog"));
			DateTime ProductsUpdateTime = Convert.ToDateTime(MySqlHelper.ExecuteScalar(MyCn, "select max(UpdateTime) from catalogs.products"));
			if ((catalogUpdate < CatalogUpdateTime) || (catalogUpdate < ProductsUpdateTime))
				if (MessageBox.Show("������� ��� �������. ���������� ���������� ��������?", "������", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					UpdateCatalog(); 
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

				catalogUpdate = DateTime.Now;
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
				if (MessageBox.Show("�� ������������� ������ ������� ��������� �������?", "������", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					List<long> selectedPrices = new List<long>();

					//������� �����-����� �� ����, �.�. ����� ��������� ���������� �������
					foreach (int rowHandle in selected)
						if (rowHandle != GridControl.InvalidRowHandle)
							selectedPrices.Add((long)gvJobs.GetDataRow(rowHandle)[JPriceItemId.ColumnName]);

					//������� �������
					foreach (long selectedPrice in selectedPrices)
					{
						MySqlTransaction tran = MyCn.BeginTransaction();
						try
						{
							MySqlCommand cmdDeleteJob = new MySqlCommand(@"
DELETE FROM 
  farm.UnrecExp
WHERE 
    PriceItemId = ?PriceItemId
AND not exists(select * from blockedprice bp where bp.PriceItemId = UnrecExp.PriceItemId)",
								MyCn, tran);
							cmdDeleteJob.Parameters.AddWithValue("?PriceItemId", selectedPrice);
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
					            WHERE PriceItemId= ?PriceItemId";
				MyCmd.Parameters.Clear();
				MyCmd.Parameters.AddWithValue("?PriceItemId", LockedPriceItemId);

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
								WHERE PriceItemId= ?PriceItemId";

				MyCmd.Parameters.Clear();
				MyCmd.Parameters.AddWithValue("?PriceItemId", LockedPriceItemId);
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

			//��������� �������� �������� �� ������ �� �����
			//������������� ���� ��������: ' ', '+', '-'
			string[] flt = Value.Split(new char[] { ' ', '+', '-' }, StringSplitOptions.RemoveEmptyEntries);

			//������ ������ �������� �� ������� �����
			List<string> firstChars = new List<string>();

			for (int i = 0; i < flt.Length; i++)
			{
				//���� ������ ����� ������ � ����� WordLen, �� ��������� ������ ������� ����� � ������
				if (flt[i].Length >= WordLen)
				{
					//������� ��������� ������� ������������ �� ������ � ����� ������
					if ((flt[i][0] == '"') || (flt[i][0] == '\''))
						flt[i] = flt[i].Substring(1, flt[i].Length - 1);

					if ((flt[i][flt[i].Length - 1] == '"') || (flt[i][flt[i].Length - 1] == '\''))
						flt[i] = flt[i].Substring(0, flt[i].Length - 1);

					//���� ����� ����� ��������� ������� WordLen-�������� � ����� � ��������� >= WordLen, �� �������� � �����
					//���� ���, �� ����� ������ ������ WordLen-������� � ������
					if (flt[i].Length - WordLen >= WordLen)
						firstChars.Add(flt[i].Substring(0, flt[i].Length - WordLen));
					else
						firstChars.Add(flt[i].Substring(0, (flt[i].Length < WordLen) ? flt[i].Length : WordLen));
				}
			}

			int positionId = 0, maxCompareCount = 0;

			//��������� �����
			for (int i = 0; i < selected.DataRowCount; i++)
			{
				//�������� ������ �� ��������
				string PropertiesValue = selected.GetDataRow(i)[FieldName].ToString();

				//��� �������� ����� ���� ������ �������, ���� ���������� �� ����������
				if (!String.IsNullOrEmpty(PropertiesValue))
				{
					int compareCount = 0;
					int currentIndex = -1;
					//������ ������� �� ������� ����� �� �����-����� ���� � ��������,
					//���� ������� ��� ����� � ��������, �� ����������� ������� ����������
					foreach (string s in firstChars)
					{
						currentIndex = PropertiesValue.IndexOf(s, StringComparison.OrdinalIgnoreCase);
						//������� � ��� ������, ���� ����� � ������ ������, ��� � ������ ������ �����, ����� ������� ����� ���� ���������� ��� �����������
						if ((currentIndex == 0) || ((currentIndex > 0) && (Char.IsSeparator(PropertiesValue[currentIndex-1]) || Char.IsPunctuation(PropertiesValue[currentIndex-1]))))
							compareCount++;
					}

					if (compareCount > maxCompareCount)
					{
						maxCompareCount = compareCount;
						positionId = i;
					}
				}
			}

			if (positionId != 0)
				selected.FocusedRowHandle = positionId;
		}

		private void ShowCatalog(int FocusedRowHandle)
		{
			DataRow dr=gvUnrecExp.GetDataRow(FocusedRowHandle);
			grpBoxCatalog2.Text = "������� �������";
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
			grpBoxCatalog2.Text = "������� ���� ��������������";
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
						if(MessageBox.Show("�������� ����������?", "������", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
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
								string Mess = String.Format("������������: {0}\r\n�����: {1}\r\n�������� ������������� �� ������������?", drCatalogName[colCatalogNameName], drCatalog[colCatalogForm]);
								if (MessageBox.Show(Mess, "������", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
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
						//���� ����� ����� ������ � ���� "FirmCr" ��������, �� ���������� ������������� �� �������������
						if ((drUN != null) && !String.IsNullOrEmpty(drUN[UEFirmCr].ToString()))
						{
							string FirmName = null;
							//���� ������������ � (UETmpCodeFirmCr is DBNull), �� �������� ���� = 0, ����� ����� �������� ���� �� ���� UETmpCodeFirmCr
							DataRow[] drFM = dtCatalogFirmCr.Select(
								"CCode = " + (Convert.IsDBNull(drUN[UETmpCodeFirmCr]) ? "0" : drUN[UETmpCodeFirmCr].ToString()));
							if (drFM.Length > 0)
							{
								FirmName = drFM[0]["CName"].ToString();
							}

							if (!String.IsNullOrEmpty(FirmName) && MessageBox.Show("�������������: " + FirmName+ "\r\n�������� ������������� �� �������������?", "������", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
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
					drUnrecExp["UETmpProductId"] = DBNull.Value;
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
					drUnrecExp["UETmpCodeFirmCr"] = DBNull.Value;
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
				//TODO: ����� ����������� ������� �������������� ������� � ������� �������������� ���������
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
				//���� ��������� ��� �������� ������� "������������� �� ��������", �� ������������� DBNull, ����� ������� ��������
				if ((long)drCatalogFirmCr["CCode"] == 0)
					drUnrecExp["UETmpCodeFirmCr"] = DBNull.Value;
				else
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
			//������� ������ ��� ������
			if (((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || e.KeyCode == Keys.OemCloseBrackets ||
				e.KeyCode == Keys.OemOpenBrackets || e.KeyCode == Keys.OemSemicolon || e.KeyCode == Keys.OemQuotes ||
				e.KeyCode == Keys.Oemcomma || e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.OemQuestion ||
				(e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9))
				&& (FocusedView.ActiveFilter.Count > 0))
			{
				FocusedView.ActiveFilter.Clear();
			}

			//�������� ������
			if (e.KeyCode == Keys.A && e.Control)
				FocusedView.ActiveFilter.Clear();

			if (e.KeyCode == Keys.Escape)
			{
				if (FocusedView.ParentView == null)
				{
					//������ Escape � ����� (������������), �� ���� � ��������� �������������� �������
					ClearCatalogGrid();
					GoToNextUnrecExp(gvUnrecExp.FocusedRowHandle + 1);
				}
				else
				{
					GridView Parent = (GridView)FocusedView.ParentView;
					//����������� �� ������� �����
					Parent.CollapseMasterRow(Parent.FocusedRowHandle);
					Parent.ZoomView();
				}
			}

			if (String.IsNullOrEmpty(FocusedView.LevelName))
			{
				//��������� ������� ������ : �����
				//������ Enter, ������ ���� �� ������� ����
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


				//�������� ������� ��� ����������� (��������������)
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
								//���� ��� ������ �������, �� ������ ������������ � ������ ���������								
								if ((bv.DataRowCount == 1) && (bv.GetDataRow(0)["Properties"] is DBNull))
								{
									//���������� �������������
									DoSynonym(e.Shift);
									ChangeBigName(gvUnrecExp.FocusedRowHandle);
								}
								else
								{
									//������������� ������� �� ����� ���������� ��������.
									GotoCatalogPosition(bv, GetFullUnrecName(gvUnrecExp.FocusedRowHandle), "Properties");
								}
								colProperties.Caption = colFForm.Caption + " - " + drCatalog[colFForm.FieldName].ToString();
							}
						}
					}
				}
				else
					if (FocusedView.LevelName == gvProducts.LevelName)
					{
						//��������� �������� ������ : ��������� �� ����������

						if (e.KeyCode == Keys.Enter)
							if (FocusedView.FocusedRowHandle != GridControl.InvalidRowHandle)
							{
								//���������� �������������
								DoSynonym(e.Shift);
								ChangeBigName(gvUnrecExp.FocusedRowHandle);
							}
					}
		}

		private void ClearCatalogGrid()
		{
			UnrecExpGridControl.Focus();
			grpBoxCatalog2.Text = "�������";
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
			//			MessageBox.Show("��������� ������������� �� �������������");
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
					//������� ������ �������������� ���������
				case Keys.Enter:
					if (gvJobs.FocusedRowHandle != GridControl.InvalidRowHandle)
						LockJob();
					break;

					//��������� ������� ������� �������
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

		//��������� ������
		private void LockJob()
		{
			JobsGridFill();
			if (gvJobs.FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvJobs.GetDataRow(gvJobs.FocusedRowHandle);
				if (dr[colJBlockBy.FieldName].ToString() == String.Empty || dr[colJBlockBy.FieldName].ToString() == Environment.UserName)
				{
					LockedPriceCode = Convert.ToInt64(dr[JPriceCode.ColumnName]);
					LockedPriceItemId = Convert.ToInt64(dr[JPriceItemId.ColumnName]);
                    PriceFMT = dr[JPriceFMT].ToString();
                    FileExt = dr[JExt].ToString();
                    if (dr[JParentSynonym] is DBNull)
						LockedSynonym = LockedPriceCode;
					else
						LockedSynonym = Convert.ToInt64(dr[JParentSynonym]);
					LockedInBlockedPrice(LockedPriceItemId, Environment.UserName);
					grpBoxCatalog2.Text = "�������";

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
						btnHideUnformFirmCr.Text = "������ �������������� ������ �� �������������";
					}
					else
						btnHideUnformFirmCr.Text = "�������� ���";

					UnrecExpGridControl.Focus();
					gvUnrecExp.FocusedRowHandle = GridControl.InvalidRowHandle;
					gvUnrecExp.FocusedRowHandle = 0;
					GoToNextUnrecExp(0);
					sbpAll.Text = String.Format("����� ����������: {0}", dtUnrecExp.Rows.Count);
				}
			}
		}

		//������������ ������
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
					LocateJobs(LockedPriceItemId, null);
					UnLockedInBlockedPrice(LockedPriceItemId);
					LockedPriceCode = -1;
					LockedPriceItemId = -1;
                    PriceFMT = String.Empty;
                    FileExt = String.Empty;
                    LockedSynonym = -1;
					gvUnrecExp.FocusedRowHandle = GridControl.InvalidRowHandle;
					dtUnrecExp.Clear();
					//��������� ������� �������
					JobsGridFill();
					sbpAll.Text = String.Empty;
					sbpCurrent.Text = String.Empty;
					this.Text = "�������� �������������� ���������";
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
				//MessageBox.Show("������� ���������� ThreadAbortException : {0}", e.ToString());
			}
			catch(Exception e)
			{
				if (f != null)
					f.Error = String.Format("������� ���������� : {0}", e.ToString());
			}
		}

		delegate bool ShowRetransPriceDelegate();

		private bool ShowRetransPrice()
		{
			string str = String.Empty;
			str = String.Format(
@"�������:
	����������� ��������� - {0}
���������:
	�� ������������ - {1}
	�� ������������� - {2}
��������� ������� ���������: {3}
��������� ������������� ���������: {4}
��������� ������� ��������� ��������������: {5}

������������ �����?", ForbiddenCount, SynonymCount, SynonymFirmCrCount, HideSynonymCount, DuplicateSynonymCount, HideSynonymFirmCrCount);
			return (MessageBox.Show(str, "������", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes);
		}

		private void ApplyChanges()
		{
			bool res = false;
			//������� �� ������������ ��������
			bool HasParentSynonym = LockedSynonym != LockedPriceCode;
			DateTime now = DateTime.Now;
			f.Status = "���������� ������...";

			//������ �������, ������� ����� ������������
			List<RetransedPrice> RetransedPriceList = new List<RetransedPrice>();

			//������� ����� ���� ��������, ������� ���������� ������������ ��������
			DataSet dsInerPrices = MySqlHelper.ExecuteDataset(MyCn, @"
select
  pc.PriceItemId,
  pf.FileExtention
from
  usersettings.pricesdata pd,
  usersettings.clientsdata cd,
  usersettings.pricescosts pc,
  usersettings.priceitems pim,
  farm.formrules fr,
  farm.pricefmts pf
where
    ((pd.PriceCode = ?LockedSynonym) or (pd.ParentSynonym = ?LockedSynonym))
and pd.AgencyEnabled = 1
and cd.FirmCode = pd.FirmCode
and cd.FirmStatus = 1
and pc.PriceCode = pd.PriceCode
and ((pd.CostType = 1) or (pc.BaseCost = 1))
and pim.Id = pc.PriceItemId
and (pim.UnformCount > 0)
and fr.Id = pim.FormRuleId
and pf.Id = fr.PriceFormatId",
				new MySqlParameter("?LockedSynonym", LockedSynonym));

			//���� � ������ ������ ����� ������, �� ��������� �� � ������
			if (dsInerPrices.Tables[0].Rows.Count > 0)
			{
				HasParentSynonym = true;
				foreach(DataRow drInerPrice in dsInerPrices.Tables[0].Rows)
					if ((LockedPriceItemId != Convert.ToInt64(drInerPrice["PriceItemId"])) && !RetransedPriceList.Exists(delegate(RetransedPrice value) { return value.PriceItemId == Convert.ToInt64(drInerPrice["PriceItemId"]); }))
						RetransedPriceList.Add(
							new RetransedPrice(
								Convert.ToInt64(drInerPrice["PriceItemId"]),
								drInerPrice["FileExtention"].ToString()));
			}

			RetransedPriceList.Insert(0, new RetransedPrice(LockedPriceItemId, FileExt));			

			SynonymCount = 0; 
			SynonymFirmCrCount = 0;
			ForbiddenCount = 0;
			HideSynonymCount = 0;
			HideSynonymFirmCrCount = 0;
			DuplicateSynonymCount = 0;

			//���-�� ��������� ������� - ���� ��� ����� ���-�� �������������� �������, �� ����� ������������� ����������
			int DelCount = 0;
			
			f.Pr = 1;
			//���������� ������ ����� ��������

			//��������� ������� �������������� ������������ ��� ����������
			MySqlDataAdapter daUnrecUpdate = new MySqlDataAdapter("select * from farm.UnrecExp where PriceItemId = ?PriceItemId", MyCn);
			MySqlCommandBuilder cbUnrecUpdate = new MySqlCommandBuilder(daUnrecUpdate);
			daUnrecUpdate.SelectCommand.Parameters.AddWithValue("?PriceItemId", LockedPriceItemId);
			DataTable dtUnrecUpdate = new DataTable();
			daUnrecUpdate.Fill(dtUnrecUpdate);
			dtUnrecUpdate.Constraints.Add("UnicNameCode", dtUnrecUpdate.Columns["RowID"], true);

			//��������� ������� ��������� ������������
			MySqlDataAdapter daSynonym = new MySqlDataAdapter("select * from farm.Synonym where PriceCode = ?PriceCode limit 0", MyCn);
			//MySqlCommandBuilder cbSynonym = new MySqlCommandBuilder(daSynonym);
			daSynonym.SelectCommand.Parameters.AddWithValue("?PriceCode", LockedSynonym);
			DataTable dtSynonym = new DataTable();
			daSynonym.Fill(dtSynonym);
			dtSynonym.Constraints.Add("UnicNameCode", dtSynonym.Columns["Synonym"], false);
			dtSynonym.Columns.Add("ChildPriceCode", typeof(long));
			daSynonym.InsertCommand = new MySqlCommand(
				@"
insert into farm.synonym (PriceCode, Synonym, Junk, ProductId) values (?PriceCode, ?Synonym, ?Junk, ?ProductId);
insert into logs.synonymlogs (LogTime, OperatorName, OperatorHost, Operation, SynonymCode, PriceCode, Synonym, Junk, ProductId, ChildPriceCode)
  values (now(), ?OperatorName, ?OperatorHost, 0, last_insert_id(), ?PriceCode, ?Synonym, ?Junk, ?ProductId, ?ChildPriceCode)", MyCn);
			daSynonym.InsertCommand.Parameters.AddWithValue("?OperatorName", Environment.UserName);
			daSynonym.InsertCommand.Parameters.AddWithValue("?OperatorHost", Environment.MachineName);
			daSynonym.InsertCommand.Parameters.Add("?PriceCode", MySqlDbType.UInt64, 0, "PriceCode");
			daSynonym.InsertCommand.Parameters.Add("?Synonym", MySqlDbType.VarString, 0, "Synonym");
			daSynonym.InsertCommand.Parameters.Add("?Junk", MySqlDbType.Byte, 0, "Junk");
			daSynonym.InsertCommand.Parameters.Add("?ProductId", MySqlDbType.UInt64, 0, "ProductId");
			daSynonym.InsertCommand.Parameters.Add("?ChildPriceCode", MySqlDbType.Int64, 0, "ChildPriceCode");
			
			f.Pr += 1;
			//��������� ������� ��������� ��������������
			MySqlDataAdapter daSynonymFirmCr = new MySqlDataAdapter("select * from farm.SynonymFirmCr where PriceCode = ?PriceCode limit 0", MyCn);
			//MySqlCommandBuilder cbSynonymFirmCr = new MySqlCommandBuilder(daSynonymFirmCr);
			daSynonymFirmCr.SelectCommand.Parameters.AddWithValue("?PriceCode", LockedSynonym);
			DataTable dtSynonymFirmCr = new DataTable();
			daSynonymFirmCr.Fill(dtSynonymFirmCr);
			dtSynonymFirmCr.Constraints.Add("UnicNameCode", new DataColumn[] {dtSynonymFirmCr.Columns["Synonym"]}, false);
			dtSynonymFirmCr.Columns.Add("ChildPriceCode", typeof(long));
			daSynonymFirmCr.InsertCommand = new MySqlCommand(
				@"
insert into farm.synonymFirmCr (PriceCode, CodeFirmCr, Synonym) values (?PriceCode, ?CodeFirmCr, ?Synonym);
insert into logs.synonymFirmCrLogs (LogTime, OperatorName, OperatorHost, Operation, SynonymFirmCrCode, PriceCode, CodeFirmCr, Synonym, ChildPriceCode) 
  values (now(), ?OperatorName, ?OperatorHost, 0, last_insert_id(), ?PriceCode, ?CodeFirmCr, ?Synonym, ?ChildPriceCode)", 
				MyCn);
			daSynonymFirmCr.InsertCommand.Parameters.AddWithValue("?OperatorName", Environment.UserName);
			daSynonymFirmCr.InsertCommand.Parameters.AddWithValue("?OperatorHost", Environment.MachineName);
			daSynonymFirmCr.InsertCommand.Parameters.Add("?PriceCode", MySqlDbType.UInt64, 0, "PriceCode");
			daSynonymFirmCr.InsertCommand.Parameters.Add("?Synonym", MySqlDbType.VarString, 0, "Synonym");
			daSynonymFirmCr.InsertCommand.Parameters.Add("?CodeFirmCr", MySqlDbType.UInt64, 0, "CodeFirmCr");
			daSynonymFirmCr.InsertCommand.Parameters.Add("?ChildPriceCode", MySqlDbType.Int64, 0, "ChildPriceCode");

			f.Pr += 1;
			//��������� ������� ����������� ���������
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
					
					//�������� ����� ������ � ������� ����������� ���������
					if (!MarkForbidden(i, "UEAlready") && MarkForbidden(i, "UEStatus"))
					{
						DataRow newDR = dtForbidden.NewRow();
								
						newDR["PriceCode"] = LockedPriceCode;
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
						//�������� ����� ������ � ������� ��������� ������������
						if (NotNameForm(i, "UEAlready") && !NotNameForm(i, "UEStatus"))
						{
							DataRow newDR = dtSynonym.NewRow();

							newDR["PriceCode"] = LockedSynonym;
							newDR["Synonym"] = GetFullUnrecName(i);
							newDR["ProductId"] = dr[UETmpProductId];
							newDR["Junk"] = dr[UEJunk];
							if (LockedSynonym != LockedPriceCode)
								newDR["ChildPriceCode"] = LockedPriceCode;
							try
							{
								dtSynonym.Rows.Add(newDR);
								SynonymCount += 1;
							}
							catch (ConstraintException)
							{
							}
						}

						//�������� ����� ������ � ������� ��������� ��������������
						if (NotFirmForm(i, "UEAlready") && !NotFirmForm(i, "UEStatus"))
						{
							DataRow newDR = dtSynonymFirmCr.NewRow();

							newDR["PriceCode"] = LockedSynonym;
							newDR["CodeFirmCr"] = dr[UETmpCodeFirmCr];
							newDR["Synonym"] = GetFirmCr(i);
							if (LockedSynonym != LockedPriceCode)
								newDR["ChildPriceCode"] = LockedPriceCode;
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

			f.Status = "���������� ��������� � ���� ������...";
			do
			{
				f.Pr = 30;
				MySqlTransaction tran = null;
				try
				{
					tran = MyCn.BeginTransaction(IsolationLevel.RepeatableRead);

					//��������� ������� ����� ��� ��������� ������������
					daSynonym.SelectCommand.Transaction = tran;
					DataTable dtSynonymCopy = dtSynonym.Copy();
					daSynonym.Update(dtSynonymCopy);

					f.Pr += 10;
                    
					//��������� ������� ����� ��� ��������� ��������������
					daSynonymFirmCr.SelectCommand.Transaction = tran;
					DataTable dtSynonymFirmCrCopy = dtSynonymFirmCr.Copy();
					daSynonymFirmCr.Update(dtSynonymFirmCrCopy);

					MySqlHelper.ExecuteNonQuery(MyCn, @"
update 
  usersettings.pricescosts,
  usersettings.priceitems
set
  priceitems.LastSynonymsCreation = now()
where
    pricescosts.PriceCode = ?PriceCode
and priceitems.Id = pricescosts.PriceItemId",
								new MySqlParameter("?PriceCode", LockedSynonym)); 
					f.Pr += 10;
					
					//��������� ������� ����� ��� ����������� ���������
					daForbidden.SelectCommand.Transaction = tran;
					DataTable dtForbiddenCopy = dtForbidden.Copy();
					daForbidden.Update(dtForbiddenCopy);

					f.Pr += 10;
                   
					//���������� ������� �������������� ���������
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
  PriceItemId = ?DeletePriceItem
and not Exists(select * from farm.blockedprice bp where bp.PriceItemId = ?DeletePriceItem and bp.BlockBy <> ?LockUserName)",
										new MySqlParameter("?DeletePriceItem", rp.PriceItemId),
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
					f.Error = String.Format("��� ���������� ��������� ��������� ������ : {0}\r\n", ex);
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

			SimpleLog.Log("ApplyChanges." + LockedPriceCode.ToString(), "res : {0}", res);

			bool S = DelCount == dtUnrecExp.Rows.Count;
			SimpleLog.Log("ApplyChanges." + LockedPriceCode.ToString(), "DelCount == dtUnrecExp.Rows.Count : {0}", DelCount == dtUnrecExp.Rows.Count);
			if (!S)
				S = (bool)f.Invoke( new ShowRetransPriceDelegate( ShowRetransPrice ) );
			SimpleLog.Log("ApplyChanges." + LockedPriceCode.ToString(), "ShowRetransPrice : {0}", S);

			if (res &&  S)
			{

#if DEBUG
				string rootpath = @"C:\Temp\";
#else
				string rootpath = @"\\fms\Prices\";
#endif

				f.Status = "�������������� �p����...";
				SimpleLog.Log("ApplyChanges." + LockedPriceCode.ToString(), "�������������� �p����...");
				f.Pr = 80;

				int CurrentPriceCode = 0;
				string CurrentFileName;
				do
				{
					CurrentFileName = RetransedPriceList[CurrentPriceCode].PriceItemId.ToString() + RetransedPriceList[CurrentPriceCode].FileExt;
					try
					{
						SimpleLog.Log("ApplyChanges." + LockedPriceCode.ToString(), "������������ : {0}", CurrentFileName);
						if (File.Exists(rootpath + "Base\\" + CurrentFileName))
						{
							if (!File.Exists(rootpath + "Inbound0\\" + CurrentFileName))
							{
								File.Move(rootpath + "Base\\" + CurrentFileName, rootpath + "Inbound0\\" + CurrentFileName);
								PricesRetrans(now, RetransedPriceList[CurrentPriceCode].PriceItemId);
							}
							else
								SimpleLog.Log("ApplyChanges." + LockedPriceCode.ToString(), "����� ���� � Inbound");
						}
						else
							SimpleLog.Log("ApplyChanges." + LockedPriceCode.ToString(), "����� ��� � Base");

						RetransedPriceList.RemoveAt(CurrentPriceCode);
					}
					catch (Exception e)
					{
						if (f != null)
							f.Error = String.Format("��� ����������� ����� {1} �������� ������ : {0}\r\n", e, CurrentFileName);
						SimpleLog.Log("ApplyChanges." + LockedPriceCode.ToString(), "��� ����������� ����� {1} �������� ������ : {0}", e, CurrentFileName);
						CurrentPriceCode++;
						Thread.Sleep(500);
					}
					if (CurrentPriceCode >= RetransedPriceList.Count)
						CurrentPriceCode = 0;
				}
				while(RetransedPriceList.Count > 0);

				SimpleLog.Log("ApplyChanges." + LockedPriceCode.ToString(), "�������������� �p���� ���������.");

			}

			f.Pr = 100;
		}

		private void PricesRetrans(DateTime now, long retransPriceItemId)
		{
			MySqlCommand mcInsert = new MySqlCommand();
			mcInsert.Connection = MyCn;
			mcInsert.Parameters.Clear();
			mcInsert.Parameters.AddWithValue("?RetransPriceItemId", retransPriceItemId);
			mcInsert.Parameters.AddWithValue("?UserName", Environment.UserName);
			mcInsert.Parameters.AddWithValue("?UserHost", Environment.MachineName);
			mcInsert.Parameters.AddWithValue("?Now", now);

			mcInsert.CommandText = 
					@"insert into logs.pricesretrans 
						(LogTime, 
						OperatorName,
						OperatorHost,
						PriceItemId) 
					values 
						(?Now,
						?UserName,
						?UserHost,
						?RetransPriceItemId)";
	
			mcInsert.ExecuteNonQuery();
		}

		private int UpDateUnrecExp(DataTable dtUnrecExpUpdate, DataRow drUpdated)
		{
			int DelCount = 0;

			int FULLFORM = (int)(FormMask.NameForm | FormMask.FirmForm | FormMask.CurrForm);

			if (!Convert.IsDBNull(drUpdated[UETmpProductId]))
			{
				//���������� �������� ����, ��� ������� ����� ���� ����������� �� ������� ���������� �������������
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
					//���� � �������� ������������� ���������� ������������ ������, �� ���������� �������������
					drUpdated["UETmpProductId"] = DBNull.Value;
					drUpdated["UEStatus"] = (int)((FormMask)Convert.ToByte(drUpdated["UEStatus"]) & (~FormMask.NameForm));
					HideSynonymCount++;
				}
			}

			if (!Convert.IsDBNull(drUpdated[UETmpCodeFirmCr]))
			{
				//���������� �������� ����, ��� ������� ����� ���� ����������� �� ������� ���������� �������������
				bool HidedSynonymFirmCr = Convert.ToBoolean(
					MySqlHelper.ExecuteScalar(MyCn,
					String.Format(@"
select
  Hidden
from
  farm.catalogfirmcr
where
    CodeFirmCr = {0}", drUpdated[UETmpCodeFirmCr]
						)
					)
				);
				if (HidedSynonymFirmCr)
				{
					//���� � �������� ������������� ���������� ������������ ������, �� ���������� �������������
					drUpdated[UETmpCodeFirmCr] = DBNull.Value;
					drUpdated["UEStatus"] = (int)((FormMask)Convert.ToByte(drUpdated["UEStatus"]) & (~FormMask.FirmForm));
					HideSynonymFirmCrCount++;
				}
			}


			//���������� �������� ����, ��� ������� ����� ���� ��� �������� � ������� ���������
			object SynonymExists = MySqlHelper.ExecuteScalar(MyCn, 
				"select ProductId from farm.synonym where synonym = ?Synonym and PriceCode=" + LockedSynonym.ToString(), 
				new MySqlParameter("?Synonym", String.Format("{0} {1} {2}", drUpdated["UEName1"], drUpdated["UEName2"], drUpdated["UEName3"])));
			if ((SynonymExists != null))
			{
				//���� � �������� ������������� ������� ��� ���-�� �������, �� ���������� �������������
				drUpdated["UETmpProductId"] = DBNull.Value;
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
					//���� ������ F12 �� ������� �������������� ���������, �� ��������� �������������
				case Keys.F12:
					if (tcMain.SelectedTab == tpUnrecExp)
					{
						DialogResult DRes;
						DRes = MessageBox.Show("��������� ����������?" , "������", MessageBoxButtons.YesNoCancel);
						UnlockJob(DRes);
						//��������� ������� ����� ������ �� ������������� �����-�����
						CheckCatalog();
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
					if ((selectedPrices != null) && (selectedPrices.Contains((long)dr[JPriceItemId.ColumnName])))
						gvJobs.SelectRow(i);
					if ((JCode != -1) && (JCode == (long)dr[JPriceItemId.ColumnName]))
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

		private void LockedInBlockedPrice(long lockPriceItemId, string BlockBy)
		{
			MySqlCommand mcInsert = new MySqlCommand("select * from blockedprice where PriceItemId = ?LockPriceItemId", MyCn);
			mcInsert.Parameters.Clear();
			mcInsert.Parameters.AddWithValue("?LockPriceItemId", lockPriceItemId);
			MySqlDataReader drInsert = mcInsert.ExecuteReader();
			bool NotExist = !drInsert.Read();
			drInsert.Close();
			drInsert = null;
			if (NotExist)
			{
				mcInsert.CommandText = @"insert into blockedprice (PriceItemId, BlockBy) values (?LockPriceItemId, ?BlockBy)";
				mcInsert.Parameters.AddWithValue("?BlockBy", BlockBy);
				mcInsert.ExecuteNonQuery();
			}
		}

		private void UnLockedInBlockedPrice(long unLockPriceItemId)
		{
			MySqlCommand mcInsert = new MySqlCommand("delete from blockedprice where PriceItemId = ?unLockPriceItemId", MyCn);
			mcInsert.Parameters.Clear();
			mcInsert.Parameters.AddWithValue("?unLockPriceItemId", unLockPriceItemId);
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

				sbpCurrent.Text = String.Format("������� �������: {0}", FocusedRowHandle+1);
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
				    e.DisplayText = "���";
			    else
				    e.DisplayText = "�������";
		    }
            if ((e.Column == colJNeedRetrans)||(e.Column == colJRetranced))
            {
                if (e.Value.ToString() == "1")
                    e.DisplayText = "��";
                else
                    e.DisplayText = "���";
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
			DataRow[] drs = dtJobs.Select("JPriceItemId = " + LockedPriceItemId.ToString());

			if (drs.Length > 0)
			{
				DataRow dr = drs[0];

				ArrayList NameArray = new ArrayList();

				foreach(DataRow UEdr in dtUnrecExp.Rows)
				{
					if ( ((FormMask)Convert.ToByte(UEdr["UEStatus"]) & FormMask.MarkForb) == FormMask.MarkForb )
					{
						string tmp = (UEdr["UECode"].ToString() + " " +UEdr["UEName1"].ToString() + " " + UEdr["UEFirmCr"].ToString()).Trim();
						if (!NameArray.Contains(tmp))
						{
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
			DataRow[] drs = dtJobs.Select("JPriceItemId = " + LockedPriceItemId.ToString());

			if (drs.Length > 0)
			{
				DataRow dr = drs[0];

				Dictionary<string, string> UnrecFirmCr = new Dictionary<string, string>();

				foreach(DataRow UEdr in dtUnrecExp.Rows)
				{
					if ( ((FormMask)Convert.ToByte(UEdr["UEStatus"]) & FormMask.FirmForm) != FormMask.FirmForm )
					{
						string tmp = UEdr["UEFirmCr"].ToString().Trim();
						if (!UnrecFirmCr.ContainsKey(tmp))
							UnrecFirmCr.Add(tmp, UEdr["UEName1"].ToString().Trim());
					}
				}


				List<string> UnrecFirmCrAndNameList = new List<string>();
				foreach (string key in UnrecFirmCr.Keys)
					UnrecFirmCrAndNameList.Add(UnrecFirmCr[key] + "  -  " + key);

				string UnrecFirmCrString = String.Join("\r\n", UnrecFirmCrAndNameList.ToArray());

				Clipboard.SetDataObject(UnrecFirmCrString);

				string subject = String.Format(UEEditor.Properties.Settings.Default.AboutFirmSubject, dr["FirmShortName"]);

				string body = "";
                body = UEEditor.Properties.Settings.Default.AboutFirmBody;

				body = String.Format(body, dr["FirmShortName"]);

				System.Diagnostics.Process.Start(String.Format("mailto:{0}?cc={1}&Subject={2}&Body={3}", GetContactText((long)dr[JFirmCode.ColumnName], 2, 0), "pharm@analit.net", subject, body));
			}
		}

		/// <summary>
		/// �������� ����� ��������� �� ����
		/// </summary>
		/// <param name="FirmCode">��� ����������</param>
		/// <param name="ContactGroupType">��� ���������� ������: 0 - General, 1 - ClientManager, 2 - OrderManager, 3 - Accountant</param>
		/// <param name="ContactType">��� ��������: 0 - Email, 1 - Phone</param>
		/// <returns>����� ���������, ����������� ";"</returns>
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
				e.DisplayText = ((byte)e.Value == 1) ? "��" : "���";
		}

		private void gvProducts_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
		{
			if ((e.Column.Name == colProperties.Name) && (e.Value is DBNull))
				e.DisplayText = "[�� ������������]";
		}

		private void gcFirmCr_KeyDown(object sender, KeyEventArgs e)
		{
			//����� ����� ��������� �������������
			//������� ������ ��� ������
			if (((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || e.KeyCode == Keys.OemCloseBrackets ||
				e.KeyCode == Keys.OemOpenBrackets || e.KeyCode == Keys.OemSemicolon || e.KeyCode == Keys.OemQuotes ||
				e.KeyCode == Keys.Oemcomma || e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.OemQuestion ||
				(e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9))
				&& (gvFirmCr.ActiveFilter.Count > 0))
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

				if ((dr != null) && (Convert.ToInt64(dr[colCatalogProductsCount.ColumnName]) > 0))
					e.Appearance.BackColor = Color.LightGreen;
			}

		}

		private void gvCatForm_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
		{
			if ((e.Info.IsRowIndicator) && (e.RowHandle != GridControl.InvalidRowHandle) && (e.RowHandle != -1))
			{
				DataRow dr = ((GridView)sender).GetDataRow(e.RowHandle);

				if ((dr != null) && (Convert.ToInt64(dr[colCatalogProductsCount.ColumnName]) > 0))
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
				btnHideUnformFirmCr.Text = "�������� ���";
			}
			else
			{
				dtUnrecExp.DefaultView.RowFilter = null;
				btnHideUnformFirmCr.Text = "������ �������������� ������ �� �������������";
			}

			UnrecExpGridControl.Focus();
		}

	}

	public class RetransedPrice
	{
		public long PriceItemId;
		public string FileExt;

		public RetransedPrice(long priceItemId, string AFileExt)
		{
			this.PriceItemId = priceItemId;
			this.FileExt = AFileExt;
		}
	}

	internal class UEEditorExceptionHandler
	{

		// Handles the exception event.
		public static void OnThreadException(object sender, System.Threading.ThreadExceptionEventArgs t)
		{
			try
			{
				System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
					"service@analit.net",
					"service@analit.net",
					"�������������� ������ � UEEditor",
String.Format(@"
��������     = {0}
������������ = {1}
���������    = {2}
������       =
{3}",
						sender,
						Environment.UserName,
						Environment.MachineName,
						t.Exception)); 
				System.Net.Mail.SmtpClient sm = new System.Net.Mail.SmtpClient(UEEditor.Properties.Settings.Default.SMTPHost);
				sm.Send(m);
			}
			catch
			{ }
			MessageBox.Show("� ���������� �������� �������������� ������.\r\n���������� �� ������ ���� ���������� ������������.");
		}

	}
}
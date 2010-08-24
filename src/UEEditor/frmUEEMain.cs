using System;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using System.Threading;
using System.Security.Permissions;
using Microsoft.Win32;
using RemotePriceProcessor;
using UEEditor.Properties;
using log4net;
using DevExpress.Utils.Paint;
using System.Configuration;
using UEEditor.Helpers;
using Common.MySql;
using GlobalMySql = MySql.Data.MySqlClient;
using DevExpress.XtraGrid.Views.Base;


[assembly: RegistryPermissionAttribute(SecurityAction.RequestMinimum, ViewAndModify = "HKEY_CURRENT_USER")]
namespace UEEditor
{
	[FlagsAttribute]
	public enum FormMask : byte
	{
		//Сопоставлено по наименованию
		NameForm = 1,
		//Сопоставлено по производителю
		FirmForm = 2,
		// Полностью формализован по наименованию, производителю
		FullForm = FirmForm | NameForm,
		//Помечено как запрещенное
		MarkForb = 8,
	}

	public partial class frmUEEMain : Form
	{
		private Statistics _statistics = new Statistics();
		private string BaseRegKey = "Software\\Inforoom\\UEEditor";
		private string JregKey;
		private string CregKey;
		private string UEregKey;
		private string FregKey;
		private string ZregKey;

		//Время последнего обновления каталога
		private DateTime catalogUpdate;
        
		public string PriceFMT = String.Empty;
        public string FileExt = String.Empty;
		public long LockedPriceCode = -1;
		public long LockedPriceItemId = -1;
		public long LockedSynonym = -1;
		public frmProgress formProgress = null;
		public string producerSeachText;

		public const string unknownProducer = "производитель не известен";

		private PriceProcessorWcfHelper _priceProcessor;

		public frmUEEMain()
		{
			InitializeComponent();

			_priceProcessor = new PriceProcessorWcfHelper(Settings.Default.WCFServiceUrl);

			var createExclude = new Button {
				Text = "Нет нужного производителя (F3)",
				Dock = DockStyle.Bottom,
				Visible = false
			};
			createExclude.Click += (s, a) => {
				ProducerSynonymResolver.CreateExclude(GetCurrentItem());
				GoToNextUnrecExp(gvUnrecExp.FocusedRowHandle);
			};
			pFirmCr.VisibleChanged += (s, a) => {
				createExclude.Visible = pFirmCr.Visible;
			};
			pnlCenter2.Controls.Add(createExclude);
		}

		private void frmUEEMain_Load(object sender, EventArgs e)
		{
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

			tcMain.TabPages.Remove(tpUnrecExp);
			tcMain.TabPages.Remove(tpZero);
			tcMain.TabPages.Remove(tpForb);

			// Заполняем таблицу заданий
			JobsGridFill();

			// Запоняем каталожные таблицы
			CatalogNameGridFill();

			FormGridFill();

			catalogUpdate = DateTime.Now;

			JobsGridControl.Select();
		}

		private void frmUEEMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (tcMain.SelectedTab == tpUnrecExp)
			{
				var DRes = MessageBox.Show("Вы находитесь в режиме редактирования прайс-листа. Сохранить изменения?", "Вопрос", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
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
			_priceProcessor.Dispose();
		}

		private string[] GetPriceItemIdsInQueue()
		{
#if DEBUG
			return new string[0];
#else
			try
			{
				return _priceProcessor.InboundPriceItemIds();
			}
			catch (Exception)
			{}
			return new string[0];
#endif
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
				//выбрали прайс-листы из базы, т.к. может произойти обновление таблицы
				foreach (int rowHandle in selected)
					if (rowHandle != GridControl.InvalidRowHandle)
						selectedPrices.Add((long)gvJobs.GetDataRow(rowHandle)[JPriceItemId.ColumnName]);
			}

			var priceItemIdsInQueue = GetPriceItemIdsInQueue();
			var listPriceItemIds = String.Empty;
			if (priceItemIdsInQueue.Length > 0)
			{
				listPriceItemIds = priceItemIdsInQueue[0];
				for (int i = 1; i < priceItemIdsInQueue.Length; i++)
					listPriceItemIds += "," + priceItemIdsInQueue[i];
			}
			if (listPriceItemIds.Length > 0)
				listPriceItemIds = String.Format(" and pim.Id not in ({0})", listPriceItemIds);

			With.Slave((slaveConnection) =>
			{
				var commandHelper =
					new CommandHelper(
						new MySqlCommand(@"
SELECT
        PD.FirmCode as JFirmCode,
        cd.ShortName as FirmShortName,
        pim.Id as JPriceItemId,
        PD.PriceCode As JPriceCode,
        concat(CD.ShortName, ' (', if(pd.CostType = 1, concat(pd.PriceName, ' [Колонка] ', pc.CostName), pd.PriceName), ')') as JName,
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
      COUNT(IF(unrecexp.PriorProductId is null, 1, null)) as NamePos
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
and pc.PriceItemId = pim.Id"
+
listPriceItemIds
+
@"
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
and synonymcd.FirmCode = synonympd.FirmCode"
					,
					slaveConnection));

				dtJobs.Clear();

				JobsGridControl.BeginUpdate();
				try
				{
					commandHelper.Fill(dsMain, dtJobs.TableName);
				}
				finally
				{
					JobsGridControl.EndUpdate();
				}
			});			

			LocateJobs(CurrPriceItemId, (selectedPrices.Count <= 1) ? null : selectedPrices);
			statusBar1.Panels[0].Text = "Заданий в очереди: " + dtJobs.Rows.Count;
		}

		private void UnrecExpGridFill()
		{
			With.Slave((slaveConnection) => { 
				var commandHelper = new CommandHelper(new MySqlCommand(@"SELECT RowID As UERowID,
                  Name1 As UEName1, 
				  FirmCr As UEFirmCr, 
				  Code As UECode, 
				  CodeCr As UECodeCr, 
				  Unit As UEUnit, 
				  Volume As UEVolume, 
				  Quantity As UEQuantity, 
				  Note, 
				  Period As UEPeriod, 
				  Doc, 
				  PriorProductId As UEPriorProductId,  
				  p.CatalogId As UEPriorCatalogId,
				  PriorProducerId As UEPriorProducerId, 
				  ProductSynonymId As UEProductSynonymId,  
				  ProducerSynonymId As UEProducerSynonymId, 
				  Status As UEStatus,
                  Already As UEAlready, 
				  Junk As UEJunk,
				  HandMade As UEHandMade
				  FROM farm.UnrecExp 
					left join Catalogs.Products p on p.Id = PriorProductId
				  WHERE PriceItemId= ?LockedPriceItemId ORDER BY Name1"
					,
					slaveConnection));
				commandHelper.AddParameter("?LockedPriceItemId", LockedPriceItemId);
				
				dtUnrecExp.Clear();

				UnrecExpGridControl.BeginUpdate();
				try
				{
					commandHelper.Fill(dsMain, dtUnrecExp.TableName);
					if (!dtUnrecExp.Columns.Contains("SynonymObject"))
						dtUnrecExp.Columns.Add("SynonymObject", typeof (object));
				}
				finally
				{
					UnrecExpGridControl.EndUpdate();
				}

			});			
		}

		private void CatalogNameGridFill()
		{
			With.Slave((slaveConnection) =>
			{
				var commandHelper = new CommandHelper(new MySqlCommand(@"
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
order by Name"
					,
					slaveConnection));

				dtCatalogNames.Clear();

				commandHelper.Fill(dsMain, dtCatalogNames.TableName);
			});
		}

		private void ProducersGridFillByName(string name, uint productId)
		{
			With.Slave((slaveConnection) => {

				var commandHelper = new CommandHelper(new MySqlCommand(@"
SELECT
  p.Id As CCode,
  p.Name As CName,
  1 as CIsAssortment
FROM
  catalogs.products, 
  catalogs.assortment a,
  catalogs.Producers P
  left join farm.BlockedProducerSynonyms bps on (bps.ProducerId = p.Id) and (bps.PriceCode = ?LockedSynonym) and (bps.Synonym = ?Name)
where
    (products.Id = ?ProductId)
and (a.CatalogId = products.CatalogId)
and (p.Id = a.ProducerId)
and (bps.id is null)
and (" + GetFilterString(name, "p.Name", "  ") + ") " +
		@"
union
SELECT
  p.Id As CCode,
  pe.Name As CName,
  1 as CIsAssortment
FROM
  (
  catalogs.products, 
  catalogs.assortment a,
  catalogs.Producers P,
  catalogs.ProducerEquivalents PE
  )
  left join farm.BlockedProducerSynonyms bps on (bps.ProducerId = p.Id) and (bps.PriceCode = ?LockedSynonym) and (bps.Synonym = ?Name)
where
    (products.Id = ?ProductId)
and (a.CatalogId = products.CatalogId)
and (p.Id = a.ProducerId)
and (pe.ProducerId = p.Id)
and (bps.id is null)
and (" + GetFilterString(name, "PE.Name", "  ") + ") " +
		"order by CName",slaveConnection));

				commandHelper.AddParameter("?LockedSynonym", LockedSynonym);
				commandHelper.AddParameter("?Name", name);
				commandHelper.AddParameter("?ProductId", productId);

				dtCatalogFirmCr.Clear();

				commandHelper.Fill(dsMain, dtCatalogFirmCr.TableName);
				if (dtCatalogFirmCr.Rows.Count == 0)
				{
					dtCatalogFirmCr.Clear();

					commandHelper = new CommandHelper(new MySqlCommand(@"
SELECT
  p.Id As CCode,
  p.Name As CName,
  1 as CIsAssortment
FROM
  catalogs.products, 
  catalogs.assortment a,
  catalogs.Producers P
  left join farm.BlockedProducerSynonyms bps on (bps.ProducerId = p.Id) and (bps.PriceCode = ?LockedSynonym) and (bps.Synonym = ?Name)
where
    products.Id = ?ProductId
and a.CatalogId = products.CatalogId
and p.Id = a.ProducerId
and bps.id is null

union

SELECT
  p.Id As CCode,
  pe.Name As CName,
  1 as CIsAssortment
FROM
  (
  catalogs.products, 
  catalogs.assortment a,
  catalogs.Producers P,
  catalogs.ProducerEquivalents PE
  )
  left join farm.BlockedProducerSynonyms bps on (bps.ProducerId = p.Id) and (bps.PriceCode = ?LockedSynonym) and (bps.Synonym = ?Name)
where
    (products.Id = ?ProductId)
and (a.CatalogId = products.CatalogId)
and (p.Id = a.ProducerId)
and (pe.ProducerId = p.Id)
and (bps.id is null)
order by CName", slaveConnection));
					commandHelper.AddParameter("?LockedSynonym", LockedSynonym);
					commandHelper.AddParameter("?Name", name);
					commandHelper.AddParameter("?ProductId", productId);
					commandHelper.Fill(dsMain, dtCatalogFirmCr.TableName);
				}

				//Добавляем в начало таблицы определенную запись, обозначающую понятие "производитель не известен"
				DataRow drUnknown = dtCatalogFirmCr.NewRow();
				drUnknown["CCode"] = 0;
				drUnknown["CName"] = unknownProducer;
				drUnknown[CIsAssortment.ColumnName] = true;
				dtCatalogFirmCr.Rows.InsertAt(drUnknown, 0);

			});
		}

		private void ProducersGridFillByFilter(string name, string filter, long? productId)
		{
			dtCatalogFirmCr.Clear();
			With.Slave((slaveConnection) => {
				long catalogId = Convert.ToInt64(GlobalMySql.MySqlHelper.ExecuteScalar(
					slaveConnection, 
					"select CatalogId from catalogs.products where Id = ?ProductId",
					new MySqlParameter("?ProductId", productId)));
				var commandHelper = new CommandHelper(new MySqlCommand(@"
SELECT
  p.Id As CCode,
  p.Name As CName,
  1 as CIsAssortment
FROM catalogs.Producers P
  join catalogs.assortment a on a.CatalogId = ?CatalogId and a.ProducerId = p.Id
  left join farm.BlockedProducerSynonyms bps on (bps.ProducerId = p.Id) and (bps.PriceCode = ?LockedSynonym) and (bps.Synonym = ?Name)
where p.Name like ?filter
and bps.id is null

union

SELECT
  p.Id As CCode,
  pe.Name As CName,
  1 as CIsAssortment
FROM catalogs.Producers P
  join catalogs.ProducerEquivalents PE on pe.ProducerId = p.Id
  join catalogs.assortment a on a.CatalogId = ?CatalogId and a.ProducerId = p.Id
  left join farm.BlockedProducerSynonyms bps on (bps.ProducerId = p.Id) and (bps.PriceCode = ?LockedSynonym) and (bps.Synonym = ?Name)
where  pe.Name like ?filter
and (bps.id is null)
order by CName", slaveConnection));

				commandHelper.AddParameter("?LockedSynonym", LockedSynonym);
				commandHelper.AddParameter("?Name", name);
				commandHelper.AddParameter("?filter", "%" + filter + "%");
				commandHelper.AddParameter("?CatalogId", catalogId);


				dtCatalogFirmCr.Clear();

				commandHelper.Fill(dsMain, dtCatalogFirmCr.TableName);

				//Добавляем в начало таблицы определенную запись, обозначающую понятие "производитель не известен"
				DataRow drUnknown = dtCatalogFirmCr.NewRow();
				drUnknown["CCode"] = 0;
				drUnknown["CName"] = unknownProducer;
				drUnknown[CIsAssortment.ColumnName] = true;
				dtCatalogFirmCr.Rows.InsertAt(drUnknown, 0);
			});
		}

		private void FormGridFill()
		{
			With.Slave((slaveConnection) =>
			{
				var commandHelper = new CommandHelper(new MySqlCommand(@"
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
order by Form"
					,
					slaveConnection));

				commandHelper.Fill(dsMain, dtCatalog.TableName);
			});
		}

		private void ProductsFill(ulong CatalogId)
		{
			With.Slave((slaveConnection) =>
			{
				var commandHelper = new CommandHelper(new MySqlCommand(@"
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
",
					slaveConnection));

				commandHelper.AddParameter("?CatalogId", CatalogId);

				commandHelper.Fill(dsMain, dtProducts.TableName);
			});
		}

		private void ProductsFillByProductId(ulong ProductId)
		{
			With.Slave((slaveConnection) =>
			{
				var commandHelper = new CommandHelper(new MySqlCommand(@"
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
",
					slaveConnection));

				commandHelper.AddParameter("?ProductId", ProductId);

				commandHelper.Fill(dsMain, dtProducts.TableName);
			});
		}

		private void CheckCatalog()
		{
			DateTime CatalogUpdateTime = DateTime.Now;
			DateTime ProductsUpdateTime = DateTime.Now;

			With.Slave((slaveConnection) =>
			{
				CatalogUpdateTime = Convert.ToDateTime(
					GlobalMySql.MySqlHelper.ExecuteScalar(slaveConnection, "select max(UpdateTime) from catalogs.catalog"));
				ProductsUpdateTime = Convert.ToDateTime(
					GlobalMySql.MySqlHelper.ExecuteScalar(slaveConnection, "select max(UpdateTime) from catalogs.products"));
			});

			if ((catalogUpdate < CatalogUpdateTime) || (catalogUpdate < ProductsUpdateTime))
				if (MessageBox.Show("Каталог был изменен. Произвести обновление каталога?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					UpdateCatalog(); 
		}

		private void UpdateCatalog()
		{
			CatalogGridControl.BeginUpdate();
			try
			{
				dtProducts.Clear();
				dtCatalog.Clear();

				CatalogNameGridFill();

				FormGridFill();

				catalogUpdate = DateTime.Now;
			}
			finally
			{
				CatalogGridControl.EndUpdate();
			}
		}

		private void btnDelJob_Click(object sender, EventArgs e)
		{
			int[] selected = gvJobs.GetSelectedRows();
			if ((selected != null) && (selected.Length > 0))
			{
				if (MessageBox.Show("Вы действительно хотите удалить выбранные задания?", 
					"Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question, 
					MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					try
					{
						List<long> selectedPrices = new List<long>();

						//выбрали прайс-листы из базы, т.к. может произойти обновление таблицы
						foreach (int rowHandle in selected)
							if (rowHandle != GridControl.InvalidRowHandle)
							{
								var row = gvJobs.GetDataRow(rowHandle);
								if (row != null)
									selectedPrices.Add((long) (row[JPriceItemId.ColumnName]));
							}

						With.Transaction(
							(connection, transaction) =>
								{
									MySqlCommand cmdDeleteJob =
										new MySqlCommand(
											@"
DELETE FROM 
  farm.UnrecExp
WHERE 
    PriceItemId = ?PriceItemId
AND not exists(select * from blockedprice bp where bp.PriceItemId = UnrecExp.PriceItemId)",
											connection, transaction);
									cmdDeleteJob.Parameters.Add("?PriceItemId", MySqlDbType.Int64);

									//удаляем задания
									foreach (long selectedPrice in selectedPrices)
									{
										cmdDeleteJob.Parameters["?PriceItemId"].Value = selectedPrice;
										cmdDeleteJob.ExecuteNonQuery();
									}
								}
							);
					}
					catch (Exception ex)
					{
						Mailer.SendMessageToService(ex);
						MessageBox.Show("Невозможно удалить выбранные задания. Информация об ошибке отправлена разработчику.",
							"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);	
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
				if ((dr[colJBlockBy.FieldName].ToString() == String.Empty) || dr[colJBlockBy.FieldName].ToString().Equals(Environment.UserName.ToLower(), StringComparison.OrdinalIgnoreCase))
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

				With.Slave((slaveConnection) =>
				{
					var commandHelper = new CommandHelper(new MySqlCommand(@"
SELECT 
  Forb As FForb 
FROM 
  farm.Forb 
WHERE PriceItemId= ?PriceItemId",
						slaveConnection));

					commandHelper.AddParameter("?PriceItemId", LockedPriceItemId);

					dtForb.Clear();
					commandHelper.Fill(dsMain, dtForb.TableName);
				});
			}

			if (tcMain.SelectedTab == tpZero)
			{
				ZeroGridControl.Select();

				With.Slave((slaveConnection) =>
				{
					var commandHelper = new CommandHelper(new MySqlCommand(@"
SELECT 
  Forb As FForb 
FROM 
  farm.Forb 
WHERE PriceItemId= ?PriceItemId",
						slaveConnection));

					commandHelper.AddParameter("?PriceItemId", LockedPriceItemId);

					dtZero.Clear();
					commandHelper.Fill(dsMain, dtZero.TableName);
				});
			}

     	}

		private FormMask GetMask(int NumRow, string FieldName)
		{
			DataRow dr = gvUnrecExp.GetDataRow(NumRow);
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

		private bool MarkForbidden(int NumRow, string FieldName)
		{
			FormMask m = GetMask(NumRow, FieldName);
			return (m & FormMask.MarkForb) == FormMask.MarkForb;
		}

		private string GetFilterString(string Value, string FieldName)
		{
			return GetFilterString(Value, FieldName, "[]");
		}

		private string GetFilterString(string Value, string FieldName, string fieldQuote)
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
			return fieldQuote[0] + FieldName + fieldQuote[1] + " like '" + String.Join("%' or " + fieldQuote[0] + FieldName + fieldQuote[1] + " like '", flt2) + "%'";
		}

		private void GotoCatalogPosition(GridView selected, string Value, string FieldName)
		{
			GotoCatalogPosition(selected, Value, FieldName, false, null);
		}

		private void GotoCatalogPosition(GridView selected, string Value, string FieldName, bool skipBlocked, string blockedFieldName)
		{
			int WordLen = 3;

			//Разбиваем входящие значение из прайса на слова
			//разделителями слов являются: ' ', '+', '-'
			string[] flt = Value.Split(new char[] { ' ', '+', '-' }, StringSplitOptions.RemoveEmptyEntries);

			//массив первых символов из каждого слова
			List<string> firstChars = new List<string>();

			for (int i = 0; i < flt.Length; i++)
			{
				//Если длинна слова больше и равна WordLen, то добавляем первые символы слова в массив
				if (flt[i].Length >= WordLen)
				{
					//удаляем возможные символы квотирование из начала и конца строки
					if ((flt[i][0] == '"') || (flt[i][0] == '\''))
						flt[i] = flt[i].Substring(1, flt[i].Length - 1);

					if ((flt[i][flt[i].Length - 1] == '"') || (flt[i][flt[i].Length - 1] == '\''))
						flt[i] = flt[i].Substring(0, flt[i].Length - 1);

					//Если длина слова позволяет вычесть WordLen-символов с конца и останется >= WordLen, то обрезаем с конца
					//Если нет, то берем только первые WordLen-символы с начала
					if (flt[i].Length - WordLen >= WordLen)
						firstChars.Add(flt[i].Substring(0, flt[i].Length - WordLen));
					else
						firstChars.Add(flt[i].Substring(0, (flt[i].Length < WordLen) ? flt[i].Length : WordLen));
				}
			}

			int positionId = 0, maxCompareCount = 0;

			//Произодим поиск
			for (int i = 0; i < selected.DataRowCount; i++)
			{
				//Значение строки из каталога
				string PropertiesValue = selected.GetDataRow(i)[FieldName].ToString();

				//Пробрасываем заблокированные
				if (skipBlocked && (bool)selected.GetDataRow(i)[blockedFieldName])
					continue;

				//Это значение может быть пустой строкой, если сравниваем со свойствами
				if (!String.IsNullOrEmpty(PropertiesValue))
				{
					int compareCount = 0;
					int currentIndex = -1;
					//Первые символы из каждого слова из прайс-листа ищем в каталоге,
					//если находим это слово в каталоге, то увеличиваем счетчик совпадений
					foreach (string s in firstChars)
					{
						currentIndex = PropertiesValue.IndexOf(s, StringComparison.OrdinalIgnoreCase);
						//Совпало в том случае, если нашли в начале строки, или в начале любого слова, перед которым стоит знак пунктуации или разделитель
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
			grpBoxCatalog2.Text = "Каталог товаров";
			CatalogGridControl.Visible = true;
			pFirmCr.Visible = false;

			CatalogGridControl.FocusedView = gvCatalog;
			gvCatalog.CollapseAllDetails();
			gvCatalog.ZoomView();
			gvCatalog.ActiveFilter.Clear();
			gvCatalog.ActiveFilter.Add(gvCatalog.Columns["Name"], 
				new ColumnFilterInfo( GetFilterString( GetFullUnrecName(FocusedRowHandle), "Name" ) , ""));
			if (gvCatalog.DataRowCount == 0)
				gvCatalog.ActiveFilter.Clear();
			else
				GotoCatalogPosition(gvCatalog, GetFullUnrecName(FocusedRowHandle), "Name");
		}

		private void ShowCatalogFirmCr(int FocusedRowHandle)
		{
			DataRow dr = gvUnrecExp.GetDataRow(FocusedRowHandle);
			grpBoxCatalog2.Text = "Каталог фирм производителей";
			producerSeachText = String.Empty;
			pFirmCr.Visible = true;
			CatalogGridControl.Visible = false;
			
			if (dr[UEFirmCr.ColumnName].ToString() != String.Empty)
			{
				ProducersGridFillByName(
					dr[UEFirmCr.ColumnName].ToString(),
					Convert.ToUInt32(dr[UEPriorProductId.ColumnName]));
				if (gvFirmCr.DataRowCount > 3)
					GotoCatalogPosition(gvFirmCr, dr[UEFirmCr.ColumnName].ToString(), "CName");
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
				DataRow dr = gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);
				
				if ((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.MarkForb) == FormMask.MarkForb)
				{
					CatalogGridControl.Enabled = false;
					ClearCatalogGrid();
				}
				else if ((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.NameForm) != FormMask.NameForm)
				{
					CatalogGridControl.Enabled = true;
					ShowCatalog(gvUnrecExp.FocusedRowHandle);
					CatalogGridControl.Focus();
				}
				else if ((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.FirmForm) != FormMask.FirmForm)
				{
					gcFirmCr.Enabled = true;
					ShowCatalogFirmCr(gvUnrecExp.FocusedRowHandle);
					gcFirmCr.Focus();
				}
				else if ((int)dr[UEStatus.ColumnName] == (int)FormMask.FullForm)
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
					if (((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.MarkForb) == FormMask.MarkForb) 
						&& ((GetMask(gvUnrecExp.FocusedRowHandle, "UEAlready") & FormMask.MarkForb) != FormMask.MarkForb))
						if(MessageBox.Show("Отменить запрещение?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
						{
							UnmarkUnrecExpAsForbidden(gvUnrecExp.FocusedRowHandle);
							return;
						}
						else
							return;

					if (((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.NameForm) == FormMask.NameForm)
						&& ((GetMask(gvUnrecExp.FocusedRowHandle, "UEAlready") & FormMask.NameForm) != FormMask.NameForm))
					{
						DataRow drUN = gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);
						if (drUN != null)
						{
							dtProducts.Clear();
							ProductsFillByProductId(Convert.ToUInt64(drUN[UEPriorProductId]));

							DataRow[] drProducts = dtProducts.Select("Id = " + drUN[UEPriorProductId].ToString());

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

					if (((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.FirmForm) == FormMask.FirmForm)
						&& ((GetMask(gvUnrecExp.FocusedRowHandle, "UEAlready") & FormMask.FirmForm) != FormMask.FirmForm))
					{
						DataRow drUN = gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);
						//Если нашли такую запись и поле "FirmCr" непустое, то сбрасываем сопоставление по производителю
						if ((drUN != null) && !String.IsNullOrEmpty(drUN[UEFirmCr].ToString()))
						{
							//string FirmName = null;
							object FirmName = null;
							if (Convert.IsDBNull(drUN[UEPriorProducerId]))
								FirmName = unknownProducer;
							else
							{
								//Если сопоставлено и (UEPriorProducerId is DBNull), то значение кода = 0, иначе берем значение кода из поля UEPriorProducerId
								With.Slave((slaveConnection) =>
								{
									FirmName = GlobalMySql.MySqlHelper.ExecuteScalar(slaveConnection,
									"select Name from catalogs.Producers where Id = " + drUN[UEPriorProducerId].ToString());
								});
							}

							if ((FirmName != null) && (FirmName is string) && 
								!String.IsNullOrEmpty((string)FirmName) && 
								(MessageBox.Show("Производитель: " + FirmName+ "\r\nОтменить сопоставление по производителю?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes))
							{
								UnmarkUnrecExpAsFirmForm(gvUnrecExp.FocusedRowHandle);
								flag = true;
							}
						}
					}

					if (flag)
						MoveToCatalog();
				}

				if ((e.KeyCode == Keys.F2) && ((byte)UEdr["UEHandMade"] != 1))
				{
					if (MarkUnrecExpAsForbidden(UEdr))
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
					drUnrecExp[UEStatus.ColumnName] = (int)((FormMask)Convert.ToByte(drUnrecExp[UEStatus.ColumnName]) & (~FormMask.NameForm));
					drUnrecExp[UEPriorProductId.ColumnName] = DBNull.Value;
				}
			}
			catch
			{}
		}

		private void UnmarkUnrecExpAsFirmForm(int FocusedRowHandle)
		{
			try
			{
				if ((GetMask(FocusedRowHandle, "UEStatus") & FormMask.FirmForm) == FormMask.FirmForm)
				{
					DataRow drUnrecExp = gvUnrecExp.GetDataRow(FocusedRowHandle);
					drUnrecExp[UEStatus.ColumnName] = (int)((FormMask)Convert.ToByte(drUnrecExp[UEStatus.ColumnName]) & (~FormMask.FirmForm));
					drUnrecExp[UEPriorProducerId.ColumnName] = DBNull.Value;
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
				drUnrecExp[UEStatus.ColumnName] = (int)((FormMask)Convert.ToByte(drUnrecExp[UEStatus.ColumnName]) & (~FormMask.MarkForb));
			}
		}

		private void MainTimer_Tick(object sender, System.EventArgs e)
		{
			if (tcMain.SelectedTab == tpJobs && LockedPriceCode == -1)
				JobsGridFill();
		}

		private bool MarkUnrecExpAsForbidden(DataRow drUnrecExp)
		{
			if (!Convert.IsDBNull(drUnrecExp[UEProductSynonymId.ColumnName]))
			{
				//Производим проверку того, что синоним может быть уже вставлен в таблицу синонимов
				object SynonymExists = null;
				With.Slave((slaveConnection) =>
				{
					SynonymExists = GlobalMySql.MySqlHelper.ExecuteScalar(slaveConnection,
										"select ProductId from farm.synonym where synonym = ?Synonym and PriceCode=" + LockedSynonym.ToString(),
						//todo: здесь получается фигня с добавлением пробелов в конце строки
										new MySqlParameter("?Synonym", String.Format("{0}  ", drUnrecExp["UEName1"])));
				});
				if (SynonymExists != null)
				{
					MessageBox.Show("Сопоставление как запрещенное выражение невозможно, т.к. для данного наименования существует синоним.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
			}
			if (((FormMask)Convert.ToByte(drUnrecExp[UEStatus.ColumnName]) & FormMask.MarkForb) != FormMask.MarkForb)
				drUnrecExp[UEStatus.ColumnName] = (int)((FormMask)Convert.ToByte(drUnrecExp[UEStatus.ColumnName]) | FormMask.MarkForb);
			return true;
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
			if (((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || e.KeyCode == Keys.OemCloseBrackets ||
				e.KeyCode == Keys.OemOpenBrackets || e.KeyCode == Keys.OemSemicolon || e.KeyCode == Keys.OemQuotes ||
				e.KeyCode == Keys.Oemcomma || e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.OemQuestion ||
				(e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9))
				&& (FocusedView.ActiveFilter.Count > 0))
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
						if (MarkUnrecExpAsForbidden(UEdr))
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
							ProductsFill((ulong)drCatalog[colCatalogID]);

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
			var setNext = false;
			for(int i = FromFocusHandle; i < gvUnrecExp.RowCount; i++)
			{
				if (i != GridControl.InvalidRowHandle)
				{
					var mask = GetMask(i, "UEStatus");
					if (((mask & FormMask.NameForm) != FormMask.NameForm) 
						|| ((mask & FormMask.FirmForm) != FormMask.FirmForm))
					{
						gvUnrecExp.FocusedRowHandle = i;
						setNext = true;
						break;
					}
				}
			}
			ClearCatalogGrid();
			if (setNext)
				MoveToCatalog();
		}

		private void DoSynonym(bool markAsJunk)
		{
			var bv = (GridView)CatalogGridControl.FocusedView;
			var catalog = bv.GetDataRow(bv.FocusedRowHandle);
			var productId = Convert.ToUInt32(catalog["Id"]);
			var catalogId = Convert.ToUInt32(catalog["CatalogId"]);

			ProducerSynonymResolver.UpdateStatusByProduct(GetCurrentItem(), 
				productId, 
				catalogId,
				markAsJunk);
			GoToNextUnrecExp(gvUnrecExp.FocusedRowHandle);
		}

		private DataRow GetCurrentItem()
		{
			return gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);
		}

		private void DoSynonymFirmCr()
		{
			ProducerSynonymResolver.UpdateStatusByProducer(GetCurrentItem(), 
				Convert.ToUInt32(gvFirmCr.GetDataRow(gvFirmCr.FocusedRowHandle)["CCode"]));
			GoToNextUnrecExp(gvUnrecExp.FocusedRowHandle);
		}

		private string GetFirmCr(int FocusedRowHandle)
		{
			if (FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvUnrecExp.GetDataRow(FocusedRowHandle);
				if (dr != null)
					return dr[UEFirmCr.ColumnName].ToString();
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

		// Блокирует задачу
		private void LockJob()
		{
			if (gvJobs.FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvJobs.GetDataRow(gvJobs.FocusedRowHandle);

				// Если задача не заблокирована или заблокирована текущим пользователем
				// (проверяется поле в гриде на наличие там логина)
				if ((dr[colJBlockBy.FieldName].ToString() == String.Empty) || 
					dr[colJBlockBy.FieldName].ToString().Equals(Environment.UserName.ToLower(), 
					StringComparison.OrdinalIgnoreCase) )
				{
					LockedPriceItemId = Convert.ToInt64(dr[JPriceItemId.ColumnName]);
					if (LockedInBlockedPrice(LockedPriceItemId, Environment.UserName))
					{
						LockedPriceCode = Convert.ToInt64(dr[JPriceCode.ColumnName]);
						PriceFMT = dr[JPriceFMT].ToString();
						FileExt = dr[JExt].ToString();
						if (dr[JParentSynonym] is DBNull)
							LockedSynonym = LockedPriceCode;
						else
							LockedSynonym = Convert.ToInt64(dr[JParentSynonym]);
						ProducerSynonymResolver.Init((uint) LockedSynonym);
						grpBoxCatalog2.Text = "Каталог";

						tcMain.TabPages.Add(tpUnrecExp);
						tcMain.TabPages.Add(tpZero);
						tcMain.TabPages.Add(tpForb);

						this.Text += String.Format("   --  {0}", dr[colJName.FieldName].ToString());

						tcMain.TabPages.Remove(tpJobs);

						tcMain.SelectedTab = tpUnrecExp;

						UnrecExpGridFill();

						dtUnrecExp.DefaultView.RowFilter = "UEAlready <> 1";
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
					else
						JobsGridFill();
				}
			}
		}

		// Разблокируем задачу
		private void UnlockJob(DialogResult DRes)
		{
			switch (DRes)
			{
				case DialogResult.Yes:
				{
					formProgress = new frmProgress();

					var t = new Thread(ThreadMethod);
					t.Start();

					var dr = formProgress.ShowDialog();
					formProgress = null;

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
					// Обновляем таблицу заданий
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
			using (var mainConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Main"].ConnectionString))
			{
				mainConnection.Open();
				ApplyChanges(mainConnection);
			}
			formProgress.Stop = true;
		}

		private void ApplyChanges(MySqlConnection masterConnection)
		{
			ILog _logger = LogManager.GetLogger(this.GetType());

			bool res = false;
			//Имеются ли родительские синонимы
			bool HasParentSynonym = LockedSynonym != LockedPriceCode;
			formProgress.Status = "Подготовка таблиц...";

			//Список прайсов, которые нужно перепровести
			List<RetransedPrice> RetransedPriceList = new List<RetransedPrice>();

			//Попытка найти всех потомков, которые используют родительские синонимы
			DataSet dsInerPrices = GlobalMySql.MySqlHelper.ExecuteDataset(masterConnection, @"
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
					new MySqlParameter("?LockedSynonym", LockedSynonym));;

			//Если в наборе данных будут записи, то добавляем их в список
			if (dsInerPrices.Tables[0].Rows.Count > 0)
			{				
				foreach(DataRow drInerPrice in dsInerPrices.Tables[0].Rows)
					if ((LockedPriceItemId != Convert.ToInt64(drInerPrice["PriceItemId"])) && 
						!RetransedPriceList.Exists(delegate(RetransedPrice value) { return value.PriceItemId == Convert.ToInt64(drInerPrice["PriceItemId"]); }))
						RetransedPriceList.Add(
							new RetransedPrice(
								Convert.ToInt64(drInerPrice["PriceItemId"]),
								drInerPrice["FileExtention"].ToString()));
				if (RetransedPriceList.Count > 0)
					HasParentSynonym = true;
			}

			RetransedPriceList.Insert(0, new RetransedPrice(LockedPriceItemId, FileExt));

			_statistics.Reset();

			//Кол-во удаленных позиций - если оно равно кол-во нераспознанных позиций, то прайс автоматически проводится
			int DelCount = 0;
			
			formProgress.ApplyProgress = 1;
			//Заполнение таблиц перед вставкой

			//Заполнили таблицу нераспознанных наименований для обновления
			MySqlDataAdapter daUnrecUpdate = new MySqlDataAdapter("select * from farm.UnrecExp where PriceItemId = ?PriceItemId", masterConnection);
			MySqlCommandBuilder cbUnrecUpdate = new MySqlCommandBuilder(daUnrecUpdate);
			daUnrecUpdate.SelectCommand.Parameters.AddWithValue("?PriceItemId", LockedPriceItemId);
			DataTable dtUnrecUpdate = new DataTable();
			daUnrecUpdate.Fill(dtUnrecUpdate);
			dtUnrecUpdate.Constraints.Add("UnicNameCode", dtUnrecUpdate.Columns["RowID"], true);

			//Заполнили таблицу синонимов наименований
			MySqlDataAdapter daSynonym = new MySqlDataAdapter("select * from farm.Synonym where PriceCode = ?PriceCode limit 0", masterConnection);
			daSynonym.SelectCommand.Parameters.AddWithValue("?PriceCode", LockedSynonym);
			DataTable dtSynonym = new DataTable();
			daSynonym.Fill(dtSynonym);
			dtSynonym.Constraints.Add("UnicNameCode", dtSynonym.Columns["Synonym"], false);
			dtSynonym.Columns.Add("ChildPriceCode", typeof(long));
			daSynonym.InsertCommand = new MySqlCommand(
				@"
insert into farm.synonym (PriceCode, Synonym, Junk, ProductId) values (?PriceCode, ?Synonym, ?Junk, ?ProductId);
set @LastSynonymID = last_insert_id();
insert into farm.UsedSynonymLogs (SynonymCode) values (@LastSynonymID); 
insert into logs.synonymlogs (LogTime, OperatorName, OperatorHost, Operation, SynonymCode, PriceCode, Synonym, Junk, ProductId, ChildPriceCode)
  values (now(), ?OperatorName, ?OperatorHost, 0, @LastSynonymID, ?PriceCode, ?Synonym, ?Junk, ?ProductId, ?ChildPriceCode)", masterConnection);
			daSynonym.InsertCommand.Parameters.AddWithValue("?OperatorName", Environment.UserName.ToLower());
			daSynonym.InsertCommand.Parameters.AddWithValue("?OperatorHost", Environment.MachineName);
			daSynonym.InsertCommand.Parameters.Add("?PriceCode", MySqlDbType.UInt64, 0, "PriceCode");
			daSynonym.InsertCommand.Parameters.Add("?Synonym", MySqlDbType.VarString, 0, "Synonym");
			daSynonym.InsertCommand.Parameters.Add("?Junk", MySqlDbType.Byte, 0, "Junk");
			daSynonym.InsertCommand.Parameters.Add("?ProductId", MySqlDbType.UInt64, 0, "ProductId");
			daSynonym.InsertCommand.Parameters.Add("?ChildPriceCode", MySqlDbType.Int64, 0, "ChildPriceCode");
			
			formProgress.ApplyProgress += 1;
			//Заполнили таблицу синонимов производителей
			MySqlDataAdapter daSynonymFirmCr = new MySqlDataAdapter("select sfc.* from farm.SynonymFirmCr sfc, farm.AutomaticProducerSynonyms aps where sfc.PriceCode = ?PriceCode and aps.ProducerSynonymId = sfc.SynonymFirmCrCode", masterConnection);
			daSynonymFirmCr.SelectCommand.Parameters.AddWithValue("?PriceCode", LockedSynonym);
			DataTable dtSynonymFirmCr = new DataTable();
			daSynonymFirmCr.Fill(dtSynonymFirmCr);
			dtSynonymFirmCr.Constraints.Add("UnicNameCode", new[] {dtSynonymFirmCr.Columns["Synonym"], dtSynonymFirmCr.Columns["CodeFirmCr"]}, false);
			dtSynonymFirmCr.Columns.Add("ChildPriceCode", typeof(long));
			daSynonymFirmCr.InsertCommand = new MySqlCommand(
				@"
insert into farm.synonymFirmCr (PriceCode, CodeFirmCr, Synonym) values (?PriceCode, ?CodeFirmCr, ?Synonym);
set @LastSynonymFirmCrID = last_insert_id();
insert into farm.UsedSynonymFirmCrLogs (SynonymFirmCrCode) values (@LastSynonymFirmCrID); 
", 
				masterConnection);
			var insertSynonymProducerEtalonSQL = daSynonymFirmCr.InsertCommand.CommandText;
			daSynonymFirmCr.InsertCommand.Parameters.AddWithValue("?OperatorName", Environment.UserName.ToLower());
			daSynonymFirmCr.InsertCommand.Parameters.AddWithValue("?OperatorHost", Environment.MachineName);
			daSynonymFirmCr.InsertCommand.Parameters.Add("?PriceCode", MySqlDbType.UInt64, 0, "PriceCode");
			daSynonymFirmCr.InsertCommand.Parameters.Add("?Synonym", MySqlDbType.VarString, 0, "Synonym");
			daSynonymFirmCr.InsertCommand.Parameters.Add("?CodeFirmCr", MySqlDbType.UInt64, 0, "CodeFirmCr");
			daSynonymFirmCr.InsertCommand.Parameters.Add("?ChildPriceCode", MySqlDbType.Int64, 0, "ChildPriceCode");
			daSynonymFirmCr.UpdateCommand = new MySqlCommand(
				@"
update farm.synonymFirmCr set CodeFirmCr = ?CodeFirmCr where SynonymFirmCrCode = ?SynonymFirmCrCode;
delete from farm.AutomaticProducerSynonyms where ProducerSynonymId = ?SynonymFirmCrCode;
",
				masterConnection);
			var updateSynonymProducerEtalonSQL = daSynonymFirmCr.UpdateCommand.CommandText;
			daSynonymFirmCr.UpdateCommand.Parameters.AddWithValue("?OperatorName", Environment.UserName.ToLower());
			daSynonymFirmCr.UpdateCommand.Parameters.AddWithValue("?OperatorHost", Environment.MachineName);
			daSynonymFirmCr.UpdateCommand.Parameters.Add("?PriceCode", MySqlDbType.UInt64, 0, "PriceCode");
			daSynonymFirmCr.UpdateCommand.Parameters.Add("?Synonym", MySqlDbType.VarString, 0, "Synonym");
			daSynonymFirmCr.UpdateCommand.Parameters.Add("?CodeFirmCr", MySqlDbType.UInt64, 0, "CodeFirmCr");
			daSynonymFirmCr.UpdateCommand.Parameters.Add("?ChildPriceCode", MySqlDbType.Int64, 0, "ChildPriceCode");
			daSynonymFirmCr.UpdateCommand.Parameters.Add("?SynonymFirmCrCode", MySqlDbType.Int64, 0, "SynonymFirmCrCode");

			formProgress.ApplyProgress += 1;

			formProgress.ApplyProgress += 1;
			//Заполнили таблицу запрещённых выражений
			MySqlDataAdapter daForbidden = new MySqlDataAdapter("select * from farm.Forbidden limit 0", masterConnection);
			//MySqlCommandBuilder cbForbidden = new MySqlCommandBuilder(daForbidden);
			DataTable dtForbidden = new DataTable();
			daForbidden.Fill(dtForbidden);
			dtForbidden.Constraints.Add("UnicNameCode", new DataColumn[] {dtForbidden.Columns["Forbidden"]}, false);
			daForbidden.InsertCommand = new MySqlCommand(
				@"
insert into farm.Forbidden (PriceCode, Forbidden) values (?PriceCode, ?Forbidden);
insert into logs.ForbiddenLogs (LogTime, OperatorName, OperatorHost, Operation, ForbiddenRowID, PriceCode, Forbidden) 
  values (now(), ?OperatorName, ?OperatorHost, 0, last_insert_id(), ?PriceCode, ?Forbidden);", 
				masterConnection);

			daForbidden.InsertCommand.Parameters.AddWithValue("?OperatorName", Environment.UserName.ToLower());
			daForbidden.InsertCommand.Parameters.AddWithValue("?OperatorHost", Environment.MachineName);
			daForbidden.InsertCommand.Parameters.Add("?PriceCode", MySqlDbType.UInt64, 0, "PriceCode");
			daForbidden.InsertCommand.Parameters.Add("?Forbidden", MySqlDbType.VarString, 0, "Forbidden");

			formProgress.ApplyProgress = 10;

			var forProducerSynonyms = new List<DataRow>();
			for(int i = 0; i < gvUnrecExp.RowCount; i++)
			{
				if (i == GridControl.InvalidRowHandle)
					continue;

				var dr = gvUnrecExp.GetDataRow(i);
				DelCount += UpDateUnrecExp(dtUnrecUpdate, dr, masterConnection);
					
				//Вставили новую запись в таблицу запрещённых выражений
				if (!MarkForbidden(i, "UEAlready") && MarkForbidden(i, "UEStatus"))
				{
					DataRow newDR = dtForbidden.NewRow();
								
					newDR["PriceCode"] = LockedPriceCode;
					newDR["Forbidden"] = GetFullUnrecName(i);
					try
					{
						dtForbidden.Rows.Add(newDR);
						_statistics.ForbiddenCount++;
					}
					catch(ConstraintException)
					{}
				}
				//Вставили новую запись в таблицу синонимов наименований
				else if (NotNameForm(i, "UEAlready") && !NotNameForm(i, "UEStatus"))
				{
					DataRow newDR = dtSynonym.NewRow();

					newDR["PriceCode"] = LockedSynonym;
					newDR["Synonym"] = GetFullUnrecName(i);
					newDR["ProductId"] = dr[UEPriorProductId];
					newDR["Junk"] = dr[UEJunk];
					if (LockedSynonym != LockedPriceCode)
						newDR["ChildPriceCode"] = LockedPriceCode;
					try
					{
						dtSynonym.Rows.Add(newDR);
						_statistics.SynonymCount++;
					}
					catch (ConstraintException)
					{}
				}
				//если сопоставили по производителю
				if (NotFirmForm(i, "UEAlready") && !NotFirmForm(i, "UEStatus"))
					forProducerSynonyms.Add(dr);
			}

			List<DbExclude> excludes;
			var selectExcludes = new MySqlCommand(@"select Id, CatalogId, ProducerSynonym, DoNotShow
from farm.Excludes
where pricecode = ?PriceCode", masterConnection);
			selectExcludes.Parameters.AddWithValue("?PriceCode", LockedSynonym);
			using (var reader = selectExcludes.ExecuteReader())
			{
				excludes = reader.Cast<DbDataRecord>().Select(r => new DbExclude {
					Id = Convert.ToUInt32(r["Id"]),
					CatalogId = Convert.ToUInt32(r["CatalogId"]),
					ProducerSynonym = r["ProducerSynonym"].ToString(),
					DoNotShow = Convert.ToBoolean(r["DoNotShow"]),
				}).ToList();
			}

			Updater.UpdateProducerSynonym(forProducerSynonyms, excludes, dtSynonymFirmCr, (uint) LockedSynonym, (uint) LockedSynonym, _statistics);

			var changes = dtSynonymFirmCr.GetChanges(DataRowState.Modified);
			if (changes != null)
				_statistics.SynonymFirmCrCount += changes.Rows.Count;

			formProgress.Status = "Применение изменений в базу данных...";
			With.DeadlockWraper(c => {
				var helper = new Common.MySql.MySqlHelper(masterConnection, null);
				var commandHelper = helper.Command("set @inHost = ?Host; set @inUser = ?UserName;");
				commandHelper.AddParameter("?Host", Environment.MachineName);
				commandHelper.AddParameter("?UserName", Environment.UserName.ToLower());
				commandHelper.Execute();

				//Заполнили таблицу логов для синонимов наименований
				daSynonym.SelectCommand.Connection = c;
				var dtSynonymCopy = dtSynonym.Copy();
				daSynonym.Update(dtSynonymCopy);

				formProgress.ApplyProgress += 10;

				var insertExclude = new MySqlCommand(@"
insert into Farm.Excludes(CatalogId, PriceCode, ProducerSynonym, DoNotShow) 
value (?CatalogId, ?PriceCode, ?ProducerSynonym, ?DoNotShow);", masterConnection);
				insertExclude.Parameters.AddWithValue("?PriceCode", LockedSynonym);
				insertExclude.Parameters.Add("?ProducerSynonym", MySqlDbType.VarChar);
				insertExclude.Parameters.Add("?DoNotShow", MySqlDbType.Byte);
				insertExclude.Parameters.Add("?CatalogId", MySqlDbType.UInt32);

				foreach (var exclude in excludes.Where(e => e.Id == 0))
				{
					insertExclude.Parameters["?ProducerSynonym"].Value = exclude.ProducerSynonym;
					insertExclude.Parameters["?DoNotShow"].Value = exclude.DoNotShow;
					insertExclude.Parameters["?CatalogId"].Value = exclude.CatalogId;
				}

				//Заполнили таблицу логов для синонимов производителей
				daSynonymFirmCr.SelectCommand.Connection = c;
				var dtSynonymFirmCrCopy = dtSynonymFirmCr.Copy();
				foreach (DataRow drInsertProducerSynonym in dtSynonymFirmCrCopy.Rows)
				{
					daSynonymFirmCr.InsertCommand.CommandText = insertSynonymProducerEtalonSQL;
					daSynonymFirmCr.UpdateCommand.CommandText = updateSynonymProducerEtalonSQL;

					//обновляем по одному синониму производителя, т.к. может быть добавление в исключение
					daSynonymFirmCr.Update(new[] { drInsertProducerSynonym });
				}

				GlobalMySql.MySqlHelper.ExecuteNonQuery(masterConnection, @"
update 
usersettings.pricescosts,
usersettings.priceitems
set
priceitems.LastSynonymsCreation = now()
where
pricescosts.PriceCode = ?PriceCode
and priceitems.Id = pricescosts.PriceItemId",
							new MySqlParameter("?PriceCode", LockedSynonym)); 
				formProgress.ApplyProgress += 10;
					
				//Заполнили таблицу логов для запрещённых выражений
				daForbidden.SelectCommand.Connection = c;
				var dtForbiddenCopy = dtForbidden.Copy();
				daForbidden.Update(dtForbiddenCopy);

				formProgress.ApplyProgress += 10;
				//Обновление таблицы нераспознанных выражений
				daUnrecUpdate.SelectCommand.Connection = c;
				var dtUnrecUpdateCopy = dtUnrecUpdate.Copy();
				daUnrecUpdate.Update(dtUnrecUpdateCopy);

				if (HasParentSynonym)
				{
					foreach (var rp in RetransedPriceList)
						GlobalMySql.MySqlHelper.ExecuteNonQuery(masterConnection, @"
delete
from
farm.UnrecExp
where
PriceItemId = ?DeletePriceItem
and not Exists(select * from farm.blockedprice bp where bp.PriceItemId = ?DeletePriceItem and bp.BlockBy <> ?LockUserName)",
									new MySqlParameter("?DeletePriceItem", rp.PriceItemId),
									new MySqlParameter("?LockUserName", Environment.UserName.ToLower()));
				}
				res = true;

				formProgress.ApplyProgress +=10;
			});
			
			formProgress.ApplyProgress = 80;

			formProgress.Status = String.Empty;
			formProgress.Error = String.Empty;

			NDC.Push("ApplyChanges." + LockedPriceCode);
			try
			{
				_logger.DebugFormat("res : {0}", res);

				formProgress.Status = "Перепроведение пpайса...";
				_logger.DebugFormat("Перепроведение пpайса...");
				formProgress.ApplyProgress = 80;

				while (RetransedPriceList.Count > 0)
				{
					_logger.DebugFormat("Перепроводим : {0}", RetransedPriceList[0].PriceItemId);
					try
					{
#if !DEBUG
						if (!_priceProcessor.RetransPrice(Convert.ToUInt64(RetransedPriceList[0].PriceItemId)))
						{
							_logger.DebugFormat(
								"При перепроведении priceitem {0} возникла ошибка : {1}",
								RetransedPriceList[0].PriceItemId, _priceProcessor.LastErrorMessage);
						}
#endif
					}
					catch (Exception retransException)
					{
						if (formProgress != null)
							formProgress.Error = "При перепроведении файлов возникла ошибка, которая отправлена разработчику.";
						_logger.ErrorFormat(
							"При перепроведении priceitem {0} возникла ошибка : {1}",
							RetransedPriceList[0].PriceItemId,
							retransException);
						Thread.Sleep(3000);
						Mailer.SendMessageToService(retransException);
					}
					RetransedPriceList.RemoveAt(0);
				}
				_logger.DebugFormat("Перепроведение пpайса завершено.");
			}
			finally
			{
				NDC.Pop();
			}
			formProgress.ApplyProgress = 100;
		}

		private int UpDateUnrecExp(DataTable dtUnrecExpUpdate, DataRow drUpdated, MySqlConnection masterConnection)
		{
			int DelCount = 0;

			if (!Convert.IsDBNull(drUpdated[UEPriorProductId]) &&
				CatalogHelper.IsHiddenProduct(masterConnection, Convert.ToInt64(drUpdated[UEPriorProductId])))
			{
				//Производим проверку того, что синоним может быть сопоставлен со скрытым каталожным наименованием
				//Если в процессе распознования каталожное наименование скрыли, то сбрасываем распознавание
				drUpdated[UEPriorProductId.ColumnName] = DBNull.Value;
				drUpdated[UEStatus.ColumnName] = (int)((FormMask)Convert.ToByte(drUpdated[UEStatus.ColumnName]) & (~FormMask.NameForm));
				_statistics.HideSynonymCount++;
			}

			if (Convert.IsDBNull(drUpdated[UEProductSynonymId.ColumnName]) &&
				CatalogHelper.IsSynonymExists(masterConnection, LockedSynonym, drUpdated["UEName1"].ToString()))
			{
				//Производим проверку того, что синоним может быть уже вставлен в таблицу синонимов
				//Если в процессе распознования синоним уже кто-то добавил, то сбрасываем распознавание
				drUpdated[UEPriorProductId.ColumnName] = DBNull.Value;
				drUpdated[UEStatus.ColumnName] = (int)((FormMask)Convert.ToByte(drUpdated[UEStatus.ColumnName]) & (~FormMask.NameForm));
				_statistics.DuplicateSynonymCount++;
			}

			if ((((FormMask)Convert.ToByte(drUpdated[UEAlready.ColumnName]) & FormMask.FirmForm) != FormMask.FirmForm)
				&& (((FormMask)Convert.ToByte(drUpdated[UEStatus.ColumnName]) & FormMask.FirmForm) == FormMask.FirmForm) 
				&& Convert.IsDBNull(drUpdated[UEProducerSynonymId.ColumnName])
				&& CatalogHelper.IsProducerSynonymExists(masterConnection, LockedSynonym, drUpdated[UEFirmCr.ColumnName].ToString()))
			{
				//Производим проверку того, что синоним может быть уже вставлен в таблицу синонимов
				//Если в процессе распознования синоним уже кто-то добавил, то сбрасываем распознавание
				drUpdated[UEPriorProducerId.ColumnName] = DBNull.Value;
				drUpdated[UEStatus.ColumnName] = (int)((FormMask)Convert.ToByte(drUpdated[UEStatus.ColumnName]) & (~FormMask.FirmForm));
				_statistics.DuplicateProducerSynonymCount++;
			}


			DataRow drNew = dtUnrecExpUpdate.Rows.Find( Convert.ToUInt32( drUpdated["UERowID"] ) );

			if (drNew != null)
			{
				dtUnrecExpUpdate.Rows.Remove(drNew);
				DelCount++;
			}

			return DelCount;
		}

		private void frmUEEMain_KeyDown(object sender, KeyEventArgs e)
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
						//Проверяем каталог после выхода из распознавания прайс-листа
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

		private bool LockedInBlockedPrice(long lockPriceItemId, string BlockBy)
		{
			try
			{
				var blocked = false;
				With.Transaction(
					(connection, transaction) =>
					{
						var currentBlockBy = GlobalMySql.MySqlHelper.ExecuteScalar(
							connection, "select BlockBy from blockedprice where PriceItemId = ?LockPriceItemId",
							new MySqlParameter("?LockPriceItemId", lockPriceItemId));
						if (currentBlockBy == null)
						{
							GlobalMySql.MySqlHelper.ExecuteNonQuery(connection,
								"insert into blockedprice (PriceItemId, BlockBy) values (?LockPriceItemId, ?BlockBy)",
								new MySqlParameter("?LockPriceItemId", lockPriceItemId),
								new MySqlParameter("?BlockBy", BlockBy.ToLower()));
							blocked = true;
						}
						else
							blocked = BlockBy.Equals((string)currentBlockBy, StringComparison.OrdinalIgnoreCase);
					}
					);
				return blocked;
			}
			catch (Exception exception)
			{
				ILog _logger = LogManager.GetLogger(this.GetType());
				_logger.Error("Ошибка при блокировании задания", exception);
				return false;
			}
		}

		private void UnLockedInBlockedPrice(long unLockPriceItemId)
		{
			With.Transaction(
				(connection, transaction) =>
				{
					GlobalMySql.MySqlHelper.ExecuteNonQuery(
						connection,
						"delete from blockedprice where PriceItemId = ?unLockPriceItemId",
						new MySqlParameter("?unLockPriceItemId", unLockPriceItemId));
				});
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

		private void gvUnrecExp_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
		{
			ChangeBigName(e.FocusedRowHandle);
		}

		private string GetFullUnrecName(int FocusedRowHandle)
		{
			if (FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvUnrecExp.GetDataRow(FocusedRowHandle);
				if (dr != null)
					//todo: здесь получается фигня с добавлением пробелов в конце строки
					return String.Format("{0}  ", dr["UEName1"]);
				return String.Empty;
			}
			return String.Empty;
		}

		private void gvUnrecExp_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
		{
			if (e.Column.FieldName == "UEAlready")
			{
				int v1 = (int)e.Value1;
				int v2 = (int)e.Value2;
				e.Handled = true;
				//todo: здесь происходит сортировка по статусу Already
				/*
				 * Предыдущие значения статуса
	[FlagsAttribute]
	public enum FormMask : byte
	{
		//Сопоставлено по наименованию
		NameForm = 1,
		//Сопоставлено по производителю
		FirmForm = 2,
		//Сопоставлено по валюте
		CurrForm = 4,
		//Помечено как запрещенное
		MarkForb = 8,
		//Отсутствует в ассортименте
		AssortmentAbsent = 16,
		//Помечено как исключение
		MarkExclude = 32
	}
				 * 
	[FlagsAttribute]
	public enum FormMask : byte
	{
		//Сопоставлено по наименованию
		NameForm = 1,
		//Сопоставлено по производителю
		FirmForm = 2,
		//Сопоставлено по валюте
		AssortmentForm = 4,
		// Полностью формализован по наименованию, производителю и ассортименту
		FullForm = 7, 
		//Помечено как запрещенное
		MarkForb = 8,
		// Помеченый как исключение
		MarkExclude	   = 16,
		// Формализован по наименованию, производителю и как исключение
		ExcludeForm    = 19 
	}
				 * 
				 * 
				 * 
				 * Нужно будет переписать сортировку с новыми значениями статуса
				 */
				if (v1 == v2)
					e.Result = 0;
				else
					if (v1 == 2)
				{
					if (v2 == 0)
						e.Result = 1;
					else
						e.Result = -1;
				}
				else
					if (v2 == 2)
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

		private void gvJobs_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
		{
		    if (e.Column == colJWholeSale)
		    {
			    if (e.Value.ToString() == "0")
				    e.DisplayText = "Опт";
			    else
				    e.DisplayText = "Розница";
		    }
		}

		private void UnrecExpGridControl_Click(object sender, EventArgs e)
		{
			if (gvUnrecExp.FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				CatalogGridControl.Enabled = false;
				ClearCatalogGrid();
			}
		}

		private void gvJobs_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
		{
			if (e.RowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvJobs.GetDataRow(e.RowHandle);
				if ((dr["JBlockBy"].ToString() != ""))
				{
					Rectangle  r = e.Bounds;
					r.Inflate(-1, -1);

					e.Graphics.DrawImageUnscaled(imageList1.Images[0], r.X, r.Y);
					ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
					e.Handled = true;
				}
			}		
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
					if (((FormMask)Convert.ToByte(UEdr[UEStatus.ColumnName]) & FormMask.MarkForb) == FormMask.MarkForb)
					{
						string tmp = (UEdr["UECode"].ToString() + " " +UEdr["UEName1"].ToString() + " " + UEdr[UEFirmCr.ColumnName].ToString()).Trim();
						if (!NameArray.Contains(tmp))
						{
							NameArray.Add( tmp );
						}
					}
				}

				string UnrecName = String.Join("\r\n", (string[])NameArray.ToArray(typeof(string)));

				Clipboard.SetDataObject(UnrecName);

				string subject = String.Format(Settings.Default.AboutNamesSubject, 
					dr["FirmShortName"], dr["JRegion"]);

				string body = "";
				body = Settings.Default.AboutNamesBody;

				body = String.Format(body, dr["FirmShortName"]);

				Process.Start(String.Format("mailto:{0}?cc={1}&Subject={2}&Body={3}", GetContactText((long)dr[JFirmCode.ColumnName], 2, 0), "pharm@analit.net", subject, body));
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
					if (((FormMask)Convert.ToByte(UEdr[UEStatus.ColumnName]) & FormMask.FirmForm) != FormMask.FirmForm)
					{
						string tmp = UEdr[UEFirmCr.ColumnName].ToString().Trim();
						if (!UnrecFirmCr.ContainsKey(tmp))
							UnrecFirmCr.Add(tmp, UEdr["UEName1"].ToString().Trim());
					}
				}


				List<string> UnrecFirmCrAndNameList = new List<string>();
				foreach (string key in UnrecFirmCr.Keys)
					UnrecFirmCrAndNameList.Add(UnrecFirmCr[key] + "  -  " + key);

				string UnrecFirmCrString = String.Join("\r\n", UnrecFirmCrAndNameList.ToArray());

				Clipboard.SetDataObject(UnrecFirmCrString);

				string subject = String.Format(Settings.Default.AboutFirmSubject, 
					dr["FirmShortName"], dr["JRegion"]);

				string body = "";
                body = Settings.Default.AboutFirmBody;

				body = String.Format(body, dr["FirmShortName"]);

				string mailUrl = String.Format("mailto:{0}?cc={1}&Subject={2}&Body={3}",
					GetContactText((long)dr[JFirmCode.ColumnName], 2, 0),
					"pharm@analit.net", subject, body);
				Process.Start(mailUrl); 
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
			DataSet dsContacts = null;
			With.Connection((mainConnection) => {
				dsContacts = GlobalMySql.MySqlHelper.ExecuteDataset(mainConnection, @"
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
			});
			List<string> contacts = new List<string>();
			foreach (DataRow drContact in dsContacts.Tables[0].Rows)
			{
				if (!contacts.Contains(drContact["contactText"].ToString()))
					contacts.Add(drContact["contactText"].ToString());
			}

			return String.Join("; ", contacts.ToArray());
		}

		private void gvUnrecExp_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
			if (e.RowHandle != GridControl.InvalidRowHandle)
			{
				if (e.Column == colUEColumn1)
				{
					if (((GetMask(e.RowHandle, "UEStatus") & FormMask.MarkForb) == FormMask.MarkForb))
					{
						Rectangle  r = e.Bounds;
						r.Inflate(-1, -1);
						Brush br = new SolidBrush(SystemColors.Control);
						e.Graphics.FillRectangle(br, r);
						e.Graphics.DrawImageUnscaled(imageList2.Images[3], r.X, r.Y);
						ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.Adjust);
						e.Handled = true;
					}
					else if (((GetMask(e.RowHandle, "UEStatus") & FormMask.NameForm) == FormMask.NameForm))
					{
						Rectangle  r = e.Bounds;
						r.Inflate(-1, -1);
						Brush br = new SolidBrush(SystemColors.Control);
						e.Graphics.FillRectangle(br, r);
						e.Graphics.DrawImageUnscaled(imageList2.Images[0], r.X, r.Y);
						ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.Adjust);
						e.Handled = true;
					}
				}

				if (e.Column == colUEColumn2)
				{
					if (((GetMask(e.RowHandle, "UEStatus") & FormMask.FirmForm) == FormMask.FirmForm) && 
						((GetMask(e.RowHandle, "UEStatus") & FormMask.MarkForb) != FormMask.MarkForb))
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
			}		
		}

		private void gvUnrecExp_RowCellStyle(object sender, RowCellStyleEventArgs e)
		{
			if (e.RowHandle != GridControl.InvalidRowHandle)
			{
				int i = e.RowHandle;

				DataRow UEdr = gvUnrecExp.GetDataRow(i);
				if (UEdr != null)
				{
					if (e.Column.VisibleIndex == 0)
						e.Appearance.BackColor = Color.White;
					else if (e.Column.VisibleIndex == 1)
						e.Appearance.BackColor = Color.White;
					else if (e.Column.VisibleIndex == 2)
						e.Appearance.BackColor = Color.White;
					else
					{
						if (((GetMask(i, "UEStatus") & FormMask.FirmForm) == FormMask.FirmForm) &&
						    ((GetMask(i, "UEStatus") & FormMask.NameForm) == FormMask.NameForm))
						{
							e.Appearance.BackColor = Color.Lime;
						}
						if (((GetMask(i, "UEStatus") & FormMask.MarkForb) == FormMask.MarkForb))
							e.Appearance.BackColor = SystemColors.GrayText;
						else if (((GetMask(i, "UEStatus") & FormMask.NameForm) == FormMask.NameForm))
						{
							e.Appearance.BackColor = Color.PaleGreen;
						}
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
			//if (((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || e.KeyCode == Keys.OemCloseBrackets ||
			//    e.KeyCode == Keys.OemOpenBrackets || e.KeyCode == Keys.OemSemicolon || e.KeyCode == Keys.OemQuotes ||
			//    e.KeyCode == Keys.Oemcomma || e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.OemQuestion ||
			//    (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9))
			//    && (gvFirmCr.ActiveFilter.Count > 0))
			//{
			//    gvFirmCr.ActiveFilter.Clear();
			//}

			if (!String.IsNullOrEmpty(tbProducerSearch.Text) && (e.KeyCode == Keys.Enter))
			{
				PerformProducerSearch();
				return;
			}

			if (e.KeyCode == Keys.Escape)
			{
				ClearCatalogGrid();
				GoToNextUnrecExp(gvUnrecExp.FocusedRowHandle + 1);
			}

			if (e.KeyCode == Keys.A && e.Control)
				gvFirmCr.ActiveFilter.Clear();

			if (gvFirmCr.FocusedRowHandle == GridControl.InvalidRowHandle)
				return;

			var current = GetCurrentItem();

			if (e.KeyCode == Keys.Enter)
			{
				// Если не сопоставлено по производителю
				if ((((FormMask) Convert.ToByte(current[UEStatus.ColumnName]) & FormMask.FirmForm) != FormMask.FirmForm))
				{
					DoSynonymFirmCr();
					ChangeBigName(gvUnrecExp.FocusedRowHandle);
				}
			}

			if (e.KeyCode == Keys.F2)
			{
				if ((byte)current["UEHandMade"] != 1)
				{
					if (MarkUnrecExpAsForbidden(current))
						GoToNextUnrecExp(gvUnrecExp.FocusedRowHandle + 1);
				}
			}

			if (e.KeyCode == Keys.F3)
			{
				ProducerSynonymResolver.CreateExclude(current);
				GoToNextUnrecExp(gvUnrecExp.FocusedRowHandle);
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
				dtUnrecExp.DefaultView.RowFilter = "UEAlready <> 1";
				btnHideUnformFirmCr.Text = "Показать все";
			}
			else
			{
				dtUnrecExp.DefaultView.RowFilter = null;
				btnHideUnformFirmCr.Text = "Скрыть нераспознанные только по производителю";
			}

			UnrecExpGridControl.Focus();
		}

		private void gvFirmCr_RowStyle(object sender, RowStyleEventArgs e)
		{
			if (e.RowHandle != GridControl.InvalidRowHandle)
			{
				DataRow drProducer = gvFirmCr.GetDataRow(e.RowHandle);
				if (drProducer != null)
					if (!(bool)drProducer[CIsAssortment.ColumnName])
						e.Appearance.BackColor = Color.LightGray;
			}
		}

		private void gvFirmCr_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
			if (!String.IsNullOrEmpty(producerSeachText) && 
				!String.IsNullOrEmpty(e.DisplayText) && 
				(e.DisplayText != unknownProducer))
			{
				var displayText = e.DisplayText;
				var index = displayText.IndexOf(producerSeachText, 
					StringComparison.OrdinalIgnoreCase);
				if (index == 0)
					//если найденный текст в начале строки
					e.Cache.Paint.DrawMultiColorString(e.Cache, e.Bounds, displayText, 
						displayText.Substring(index, producerSeachText.Length), 
						e.Appearance, Color.Black, Color.Yellow, false);
				else
					if (index + producerSeachText.Length == displayText.Length)
					{
						//если найденный текст в конце строки
						//должен работать вызов этого метода, но он почему-то не работает, поэтому переписано ниже
						//e.Cache.Paint.DrawMultiColorString(e.Cache, e.Bounds, displayText, displayText.Substring(0, index), e.Appearance, Color.Black, Color.Yellow, true);
						MultiColorDrawStringParams param = new MultiColorDrawStringParams(e.Appearance);
						param.Text = displayText;
						param.Bounds = e.Bounds;
						param.Ranges = new CharacterRangeWithFormat[] {
						new CharacterRangeWithFormat(0, index, e.Appearance.GetForeColor(), 
							e.Appearance.GetBackColor()),
						new CharacterRangeWithFormat(index, producerSeachText.Length, 
							Color.Black, Color.Yellow)};
						e.Cache.Paint.MultiColorDrawString(e.Cache, param);
					}
					else
					{
						//если найденный текст в середине строки
						MultiColorDrawStringParams param = new MultiColorDrawStringParams(e.Appearance);
						param.Text = displayText;
						param.Bounds = e.Bounds;
						param.Ranges = new CharacterRangeWithFormat[] {
						new CharacterRangeWithFormat(0, index, e.Appearance.GetForeColor(), 
							e.Appearance.GetBackColor()),
						new CharacterRangeWithFormat(index, producerSeachText.Length, 
							Color.Black, Color.Yellow),
						new CharacterRangeWithFormat(index+producerSeachText.Length, 
							displayText.Length-(index+producerSeachText.Length), 
							e.Appearance.GetForeColor(), e.Appearance.GetBackColor())};
						e.Cache.Paint.MultiColorDrawString(e.Cache, param);
					}
				e.Handled = true;
			}
		}

		private void ProducerSearchTimer_Tick(object sender, EventArgs e)
		{
			PerformProducerSearch();
		}

		private void tbProducerSearch_TextChanged(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(tbProducerSearch.Text))
			{
				ProducerSearchTimer.Enabled = false;
				ProducerSearchTimer.Enabled = true;
			}
		}

		private void PerformProducerSearch()
		{
			if (gvUnrecExp.FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);
				string producerSynonym = Convert.IsDBNull(dr[UEFirmCr.ColumnName]) ? String.Empty : dr[UEFirmCr.ColumnName].ToString();
				ProducerSearchTimer.Enabled = false;
				if (!String.IsNullOrEmpty(tbProducerSearch.Text))
				{
					producerSeachText = tbProducerSearch.Text;
					tbProducerSearch.Text = "";
					ProducersGridFillByFilter(
						producerSynonym,
						producerSeachText,
						Convert.ToUInt32(dr[UEPriorProductId.ColumnName]));
				}
			}
		}

		private void tbProducerSearch_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				PerformProducerSearch();
		}

		private void gcFirmCr_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!Char.IsControl(e.KeyChar))
				tbProducerSearch.Text += e.KeyChar;
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
}
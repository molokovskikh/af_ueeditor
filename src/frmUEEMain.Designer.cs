namespace UEEditor
{
	partial class frmUEEMain
	{
		private System.Data.DataSet dsMain;
		private System.Data.DataTable dtJobs;
		private System.Data.DataTable dtZero;
		private System.Data.DataTable dtForb;
		private System.Windows.Forms.StatusBarPanel statusBarPanel1;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.TabControl tcMain;
		private System.Windows.Forms.TabPage tpJobs;
		private System.Windows.Forms.TabPage tpUnrecExp;
		private System.Windows.Forms.Panel pnlTop2;
		private System.Windows.Forms.Label BigNameLabel2;
		private System.Windows.Forms.TabPage tpZero;
		private System.Windows.Forms.TabPage tpForb;
		private System.Data.DataColumn JName;
		private System.Data.DataColumn JRegion;
		private System.Data.DataColumn JPos;
		private System.Data.DataColumn JNamePos;
		private System.Data.DataColumn JJobDate;
		private System.Data.DataColumn JBlockBy;
		private System.Data.DataColumn UECode;
		private System.Data.DataColumn UECodeCr;
		private System.Data.DataColumn UEName1;
		private System.Data.DataColumn UEFirmCr;
		private System.Data.DataColumn UECurrency;
		private System.Data.DataColumn UEBaseCost;
		private System.Data.DataColumn UEUnit;
		private System.Data.DataColumn UEVolume;
		private System.Data.DataColumn UEQuantity;
		private System.Data.DataColumn UEPeriod;
		private System.Data.DataColumn UEJunk;
		private System.Data.DataColumn ZCode;
		private System.Data.DataColumn ZCodeCr;
		private System.Data.DataColumn ZName;
		private System.Data.DataColumn ZFirmCr;
		private System.Data.DataColumn ZCurrency;
		private System.Data.DataColumn ZBaseCost;
		private System.Data.DataColumn ZUnit;
		private System.Data.DataColumn ZVolume;
		private System.Data.DataColumn ZQuantity;
		private System.Data.DataColumn ZPeriod;
		private System.Data.DataColumn ZJunk;
		private System.Data.DataColumn FForb;
		private System.Data.DataColumn EUColumn1;
		private System.Data.DataColumn UEColumn2;
		private System.Data.DataColumn UEColumn3;
		private System.Data.DataTable dtUnrecExp;
		private System.ComponentModel.IContainer components;

		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private DevExpress.XtraGrid.GridControl ForbGridControl;
		private DevExpress.XtraGrid.Columns.GridColumn colFForb;
		private DevExpress.XtraGrid.GridControl ZeroGridControl;
		private DevExpress.XtraGrid.Columns.GridColumn colZCode;
		private DevExpress.XtraGrid.Columns.GridColumn colZCodeCr;
		private DevExpress.XtraGrid.Columns.GridColumn colZName;
		private DevExpress.XtraGrid.Columns.GridColumn colZFirmCr;
		private DevExpress.XtraGrid.Columns.GridColumn colZCurrency;
		private DevExpress.XtraGrid.Columns.GridColumn colZUnit;
		private DevExpress.XtraGrid.Columns.GridColumn colZVolume;
		private DevExpress.XtraGrid.Columns.GridColumn colZQuantity;
		private DevExpress.XtraGrid.Columns.GridColumn colZPeriod;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn10;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn11;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn12;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn13;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn14;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn15;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn16;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn26;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn27;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn28;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn29;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn30;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn31;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn32;
		private DevExpress.XtraGrid.Columns.GridColumn colCColumn11;
		private System.Windows.Forms.Panel pnlCenter2;
		private System.Windows.Forms.Panel pnlLeft2;
		private System.Windows.Forms.GroupBox grpBoxCatalog2;
		private DevExpress.XtraGrid.GridControl CatalogGridControl;
		private System.Windows.Forms.ImageList imageList1;
		private System.Data.DataColumn JMinReq;
		private System.Data.DataTable dtRegions;
		private System.Data.DataColumn RRegion;
		private System.Data.DataColumn JWholeSale;
		private System.Data.DataColumn JPriceCode;
		private System.Windows.Forms.Panel pnlBottom2;
		private DevExpress.XtraGrid.GridControl UnrecExpGridControl;
		private DevExpress.XtraGrid.Columns.GridColumn colUEColumn1;
		private DevExpress.XtraGrid.Columns.GridColumn colUEColumn2;
		private DevExpress.XtraGrid.Columns.GridColumn colUEColumn3;
		private DevExpress.XtraGrid.Columns.GridColumn colUECode;
		private DevExpress.XtraGrid.Columns.GridColumn colUECodeCr;
		private DevExpress.XtraGrid.Columns.GridColumn colUEName1;
		private DevExpress.XtraGrid.Columns.GridColumn colUEFirmCr;
		private DevExpress.XtraGrid.Columns.GridColumn colUECurrency;
		private DevExpress.XtraGrid.Columns.GridColumn colUEBaseCost;
		private DevExpress.XtraGrid.Columns.GridColumn colUEUnit;
		private DevExpress.XtraGrid.Columns.GridColumn colUEVolume;
		private DevExpress.XtraGrid.Columns.GridColumn colUEQuantity;
		private DevExpress.XtraGrid.Columns.GridColumn colUEPeriod;
		private DevExpress.XtraGrid.Columns.GridColumn colUEJunk;
		private System.Data.DataColumn UEStatus;
		private System.Data.DataColumn UETmpProductId;
		private System.Data.DataTable dtCatalogFirmCr;
		private System.Windows.Forms.Timer MainTimer;

		private DevExpress.XtraGrid.Views.Grid.GridView gvUnrecExp;
		private DevExpress.XtraGrid.Views.Grid.GridView gvCatalog;
		private DevExpress.XtraGrid.Views.Grid.GridView gvForb;
		private DevExpress.XtraGrid.Views.Grid.GridView gvZero;
		private System.Data.DataColumn CCodeFirmCr;
		private System.Data.DataColumn CFirmCr;
		private DevExpress.XtraGrid.Views.Grid.GridView gvCatForm;
		private DevExpress.XtraGrid.Columns.GridColumn colCName;
		private DevExpress.XtraGrid.Columns.GridColumn colFForm;
		private System.Data.DataColumn UEName2;
		private System.Data.DataColumn UEName3;
		private DevExpress.XtraGrid.Columns.GridColumn colUEStatus;
		private System.Data.DataColumn UETmpCodeFirmCr;
		private System.Data.DataColumn UEAlready;
		private DevExpress.XtraGrid.Columns.GridColumn colUEAlready;
		private System.Data.DataColumn JParentSynonym;
		private System.Data.DataColumn JPriceFMT;
		private System.Data.DataColumn UETmpCurrency;
		private System.Windows.Forms.Panel pnlCenter1;
		private System.Windows.Forms.Panel pnlWithButton1;
		private System.Windows.Forms.Panel pnlTop1;
		private System.Windows.Forms.Button btnDelJob;
		private System.Data.DataTable dtSections;
		private System.Data.DataColumn SSection;
		private System.Data.DataColumn UERowID;
		private System.Windows.Forms.ContextMenu cmUnrecExp;
		private System.Windows.Forms.MenuItem miSendAboutNames;
		private System.Windows.Forms.MenuItem miSendAboutFirmCr;
		private System.Windows.Forms.ImageList imageList2;
		private System.Windows.Forms.ColorDialog cdLegend;
		private System.Windows.Forms.GroupBox groupBox1;
		private DevExpress.XtraGrid.GridControl JobsGridControl;
		private DevExpress.XtraGrid.Views.Grid.GridView gvJobs;
		private DevExpress.XtraGrid.Columns.GridColumn colJName;
		private DevExpress.XtraGrid.Columns.GridColumn colJBlockBy;
		private DevExpress.XtraGrid.Columns.GridColumn colJRegion;
		private DevExpress.XtraGrid.Columns.GridColumn colJPos;
		private DevExpress.XtraGrid.Columns.GridColumn colJNamePos;
		private DevExpress.XtraGrid.Columns.GridColumn colJJobDate;
		private DevExpress.XtraGrid.Columns.GridColumn colJWholeSale;
		private System.Windows.Forms.Label lbJobs50Text;
		private System.Windows.Forms.Label lbJobsNamePosText;
		private System.Windows.Forms.Label lbJobsBlockText;
		private System.Data.DataColumn UEHandMade;
		private System.Windows.Forms.Button btnJobsBlock;
		private System.Windows.Forms.Button btnJobsNamePos;
		private System.Windows.Forms.Button btnJobs50;
		private System.Windows.Forms.StatusBarPanel sbpAll;
		private System.Windows.Forms.StatusBarPanel sbpCurrent;
		private System.Data.DataColumn JNeedRetrans;
		private System.Data.DataColumn JRetranced;
		private DevExpress.XtraGrid.Columns.GridColumn colJRetranced;
		private DevExpress.XtraGrid.Columns.GridColumn colJNeedRetrans;
		private System.Data.DataColumn JParentName;
		private DevExpress.XtraGrid.Columns.GridColumn colJParentName;
		private System.Data.DataColumn JExt;
		private System.Data.DataColumn JFirmCode;
		private System.Data.DataTable dtCatalogNames;
		private System.Data.DataColumn colCatalogNameID;
		private System.Data.DataColumn colCatalogNameName;
		private System.Data.DataTable dtCatalog;
		private System.Data.DataColumn colCatalogID;
		private System.Data.DataColumn colCatalog_NameID;
		private System.Data.DataColumn colCatalog_FormId;
		private System.Data.DataColumn colCatalogForm;
		private System.Data.DataTable dtProducts;
		private System.Data.DataColumn colProductId;
		private System.Data.DataColumn colProductCatalogId;
		private System.Data.DataColumn colProductProperties;
		private DevExpress.XtraGrid.Views.Grid.GridView gvProducts;
		private DevExpress.XtraGrid.Columns.GridColumn colProperties;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
			DevExpress.XtraGrid.GridLevelNode gridLevelNode2 = new DevExpress.XtraGrid.GridLevelNode();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUEEMain));
			this.gvCatForm = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colFForm = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colFProductsCount = new DevExpress.XtraGrid.Columns.GridColumn();
			this.CatalogGridControl = new DevExpress.XtraGrid.GridControl();
			this.dsMain = new System.Data.DataSet();
			this.dtJobs = new System.Data.DataTable();
			this.JPriceCode = new System.Data.DataColumn();
			this.JName = new System.Data.DataColumn();
			this.JRegion = new System.Data.DataColumn();
			this.JPos = new System.Data.DataColumn();
			this.JNamePos = new System.Data.DataColumn();
			this.JJobDate = new System.Data.DataColumn();
			this.JBlockBy = new System.Data.DataColumn();
			this.JMinReq = new System.Data.DataColumn();
			this.JWholeSale = new System.Data.DataColumn();
			this.JParentSynonym = new System.Data.DataColumn();
			this.JPriceFMT = new System.Data.DataColumn();
			this.JNeedRetrans = new System.Data.DataColumn();
			this.JRetranced = new System.Data.DataColumn();
			this.JParentName = new System.Data.DataColumn();
			this.JExt = new System.Data.DataColumn();
			this.JFirmCode = new System.Data.DataColumn();
			this.dtUnrecExp = new System.Data.DataTable();
			this.EUColumn1 = new System.Data.DataColumn();
			this.UEColumn2 = new System.Data.DataColumn();
			this.UEColumn3 = new System.Data.DataColumn();
			this.UECode = new System.Data.DataColumn();
			this.UECodeCr = new System.Data.DataColumn();
			this.UEName1 = new System.Data.DataColumn();
			this.UEFirmCr = new System.Data.DataColumn();
			this.UECurrency = new System.Data.DataColumn();
			this.UEBaseCost = new System.Data.DataColumn();
			this.UEUnit = new System.Data.DataColumn();
			this.UEVolume = new System.Data.DataColumn();
			this.UEQuantity = new System.Data.DataColumn();
			this.UEPeriod = new System.Data.DataColumn();
			this.UEJunk = new System.Data.DataColumn();
			this.UEStatus = new System.Data.DataColumn();
			this.UETmpProductId = new System.Data.DataColumn();
			this.UEName2 = new System.Data.DataColumn();
			this.UEName3 = new System.Data.DataColumn();
			this.UETmpCodeFirmCr = new System.Data.DataColumn();
			this.UEAlready = new System.Data.DataColumn();
			this.UETmpCurrency = new System.Data.DataColumn();
			this.UERowID = new System.Data.DataColumn();
			this.UEHandMade = new System.Data.DataColumn();
			this.dtZero = new System.Data.DataTable();
			this.ZCode = new System.Data.DataColumn();
			this.ZCodeCr = new System.Data.DataColumn();
			this.ZName = new System.Data.DataColumn();
			this.ZFirmCr = new System.Data.DataColumn();
			this.ZCurrency = new System.Data.DataColumn();
			this.ZBaseCost = new System.Data.DataColumn();
			this.ZUnit = new System.Data.DataColumn();
			this.ZVolume = new System.Data.DataColumn();
			this.ZQuantity = new System.Data.DataColumn();
			this.ZPeriod = new System.Data.DataColumn();
			this.ZJunk = new System.Data.DataColumn();
			this.dtForb = new System.Data.DataTable();
			this.FForb = new System.Data.DataColumn();
			this.dtRegions = new System.Data.DataTable();
			this.RRegion = new System.Data.DataColumn();
			this.dtCatalogFirmCr = new System.Data.DataTable();
			this.CCodeFirmCr = new System.Data.DataColumn();
			this.CFirmCr = new System.Data.DataColumn();
			this.dtSections = new System.Data.DataTable();
			this.SSection = new System.Data.DataColumn();
			this.dtCatalogNames = new System.Data.DataTable();
			this.colCatalogNameID = new System.Data.DataColumn();
			this.colCatalogNameName = new System.Data.DataColumn();
			this.dtCatalog = new System.Data.DataTable();
			this.colCatalogID = new System.Data.DataColumn();
			this.colCatalog_NameID = new System.Data.DataColumn();
			this.colCatalog_FormId = new System.Data.DataColumn();
			this.colCatalogForm = new System.Data.DataColumn();
			this.colCatalogProductsCount = new System.Data.DataColumn();
			this.dtProducts = new System.Data.DataTable();
			this.colProductId = new System.Data.DataColumn();
			this.colProductCatalogId = new System.Data.DataColumn();
			this.colProductProperties = new System.Data.DataColumn();
			this.gvCatalog = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colCName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gvProducts = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colProperties = new DevExpress.XtraGrid.Columns.GridColumn();
			this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.sbpAll = new System.Windows.Forms.StatusBarPanel();
			this.sbpCurrent = new System.Windows.Forms.StatusBarPanel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.tcMain = new System.Windows.Forms.TabControl();
			this.tpJobs = new System.Windows.Forms.TabPage();
			this.pnlCenter1 = new System.Windows.Forms.Panel();
			this.pnlTop1 = new System.Windows.Forms.Panel();
			this.JobsGridControl = new DevExpress.XtraGrid.GridControl();
			this.gvJobs = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colJName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colJParentName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colJWholeSale = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colJRegion = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colJNeedRetrans = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colJRetranced = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colJPos = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colJNamePos = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colJJobDate = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colJBlockBy = new DevExpress.XtraGrid.Columns.GridColumn();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnJobs50 = new System.Windows.Forms.Button();
			this.btnJobsNamePos = new System.Windows.Forms.Button();
			this.btnJobsBlock = new System.Windows.Forms.Button();
			this.lbJobs50Text = new System.Windows.Forms.Label();
			this.lbJobsNamePosText = new System.Windows.Forms.Label();
			this.lbJobsBlockText = new System.Windows.Forms.Label();
			this.pnlWithButton1 = new System.Windows.Forms.Panel();
			this.btnDelJob = new System.Windows.Forms.Button();
			this.imageList2 = new System.Windows.Forms.ImageList(this.components);
			this.tpUnrecExp = new System.Windows.Forms.TabPage();
			this.cmUnrecExp = new System.Windows.Forms.ContextMenu();
			this.miSendAboutNames = new System.Windows.Forms.MenuItem();
			this.miSendAboutFirmCr = new System.Windows.Forms.MenuItem();
			this.pnlBottom2 = new System.Windows.Forms.Panel();
			this.UnrecExpGridControl = new DevExpress.XtraGrid.GridControl();
			this.gvUnrecExp = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colUEColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUECode = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUECodeCr = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEName1 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEFirmCr = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUECurrency = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEBaseCost = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEUnit = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEVolume = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEQuantity = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEPeriod = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEJunk = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEStatus = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEAlready = new DevExpress.XtraGrid.Columns.GridColumn();
			this.pnlCenter2 = new System.Windows.Forms.Panel();
			this.pnlLeft2 = new System.Windows.Forms.Panel();
			this.grpBoxCatalog2 = new System.Windows.Forms.GroupBox();
			this.gcFirmCr = new DevExpress.XtraGrid.GridControl();
			this.gvFirmCr = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colFirmCrName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.pnlTop2 = new System.Windows.Forms.Panel();
			this.BigNameLabel2 = new System.Windows.Forms.Label();
			this.tpZero = new System.Windows.Forms.TabPage();
			this.ZeroGridControl = new DevExpress.XtraGrid.GridControl();
			this.gvZero = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colZCode = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colZCodeCr = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colZName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colZFirmCr = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colZCurrency = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colZUnit = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colZVolume = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colZQuantity = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colZPeriod = new DevExpress.XtraGrid.Columns.GridColumn();
			this.tpForb = new System.Windows.Forms.TabPage();
			this.ForbGridControl = new DevExpress.XtraGrid.GridControl();
			this.gvForb = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colFForb = new DevExpress.XtraGrid.Columns.GridColumn();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.dataGridTextBoxColumn10 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn11 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn12 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn13 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn14 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn15 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn16 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn26 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn27 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn28 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn29 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn30 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn31 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn32 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.colCColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.MainTimer = new System.Windows.Forms.Timer(this.components);
			this.cdLegend = new System.Windows.Forms.ColorDialog();
			((System.ComponentModel.ISupportInitialize)(this.gvCatForm)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.CatalogGridControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dsMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtJobs)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtUnrecExp)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtZero)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtForb)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtRegions)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtCatalogFirmCr)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtSections)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtCatalogNames)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtCatalog)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtProducts)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gvCatalog)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gvProducts)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpAll)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpCurrent)).BeginInit();
			this.panel3.SuspendLayout();
			this.tcMain.SuspendLayout();
			this.tpJobs.SuspendLayout();
			this.pnlCenter1.SuspendLayout();
			this.pnlTop1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.JobsGridControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gvJobs)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.pnlWithButton1.SuspendLayout();
			this.tpUnrecExp.SuspendLayout();
			this.pnlBottom2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.UnrecExpGridControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gvUnrecExp)).BeginInit();
			this.pnlCenter2.SuspendLayout();
			this.pnlLeft2.SuspendLayout();
			this.grpBoxCatalog2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcFirmCr)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gvFirmCr)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
			this.pnlTop2.SuspendLayout();
			this.tpZero.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ZeroGridControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gvZero)).BeginInit();
			this.tpForb.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ForbGridControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gvForb)).BeginInit();
			this.SuspendLayout();
			// 
			// gvCatForm
			// 
			this.gvCatForm.Appearance.HideSelectionRow.Options.UseBackColor = true;
			this.gvCatForm.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colFForm,
            this.colFProductsCount});
			this.gvCatForm.GridControl = this.CatalogGridControl;
			this.gvCatForm.Name = "gvCatForm";
			this.gvCatForm.OptionsBehavior.AllowIncrementalSearch = true;
			this.gvCatForm.OptionsBehavior.Editable = false;
			this.gvCatForm.OptionsDetail.ShowDetailTabs = false;
			this.gvCatForm.OptionsMenu.EnableColumnMenu = false;
			this.gvCatForm.OptionsMenu.EnableFooterMenu = false;
			this.gvCatForm.OptionsMenu.EnableGroupPanelMenu = false;
			this.gvCatForm.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.gvCatForm.OptionsView.ShowDetailButtons = false;
			this.gvCatForm.OptionsView.ShowFilterPanel = false;
			this.gvCatForm.OptionsView.ShowGroupPanel = false;
			this.gvCatForm.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gvCatForm_CustomColumnDisplayText);
			// 
			// colFForm
			// 
			this.colFForm.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.colFForm.AppearanceCell.Options.UseFont = true;
			this.colFForm.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.colFForm.AppearanceHeader.Options.UseFont = true;
			this.colFForm.Caption = "Форма выпуска";
			this.colFForm.FieldName = "Form";
			this.colFForm.Name = "colFForm";
			this.colFForm.OptionsColumn.ReadOnly = true;
			this.colFForm.Visible = true;
			this.colFForm.VisibleIndex = 0;
			this.colFForm.Width = 295;
			// 
			// colFProductsCount
			// 
			this.colFProductsCount.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.colFProductsCount.AppearanceCell.Options.UseFont = true;
			this.colFProductsCount.AppearanceCell.Options.UseTextOptions = true;
			this.colFProductsCount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.colFProductsCount.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.colFProductsCount.AppearanceHeader.Options.UseFont = true;
			this.colFProductsCount.Caption = "Свойства";
			this.colFProductsCount.FieldName = "ProductsCount";
			this.colFProductsCount.Name = "colFProductsCount";
			this.colFProductsCount.OptionsColumn.ReadOnly = true;
			this.colFProductsCount.Visible = true;
			this.colFProductsCount.VisibleIndex = 1;
			this.colFProductsCount.Width = 25;
			// 
			// CatalogGridControl
			// 
			this.CatalogGridControl.DataMember = "dtCatalogNames";
			this.CatalogGridControl.DataSource = this.dsMain;
			this.CatalogGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			// 
			// 
			// 
			this.CatalogGridControl.EmbeddedNavigator.Name = "";
			this.CatalogGridControl.Enabled = false;
			gridLevelNode1.LevelTemplate = this.gvCatForm;
			gridLevelNode2.LevelTemplate = this.gvProducts;
			gridLevelNode2.RelationName = "Products";
			gridLevelNode1.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode2});
			gridLevelNode1.RelationName = "CatalogNames";
			this.CatalogGridControl.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
			this.CatalogGridControl.Location = new System.Drawing.Point(3, 16);
			this.CatalogGridControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
			this.CatalogGridControl.LookAndFeel.UseDefaultLookAndFeel = false;
			this.CatalogGridControl.LookAndFeel.UseWindowsXPTheme = true;
			this.CatalogGridControl.MainView = this.gvCatalog;
			this.CatalogGridControl.Name = "CatalogGridControl";
			this.CatalogGridControl.Size = new System.Drawing.Size(706, 237);
			this.CatalogGridControl.TabIndex = 0;
			this.CatalogGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvCatalog,
            this.gvProducts,
            this.gvCatForm});
			this.CatalogGridControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CatalogGridControl_KeyDown);
			// 
			// dsMain
			// 
			this.dsMain.DataSetName = "NewDataSet";
			this.dsMain.Locale = new System.Globalization.CultureInfo("ru-RU");
			this.dsMain.Relations.AddRange(new System.Data.DataRelation[] {
            new System.Data.DataRelation("CatalogNames", "dtCatalogNames", "dtCatalog", new string[] {
                        "ID"}, new string[] {
                        "NameID"}, false),
            new System.Data.DataRelation("Products", "dtCatalog", "dtProducts", new string[] {
                        "ID"}, new string[] {
                        "CatalogId"}, false)});
			this.dsMain.Tables.AddRange(new System.Data.DataTable[] {
            this.dtJobs,
            this.dtUnrecExp,
            this.dtZero,
            this.dtForb,
            this.dtRegions,
            this.dtCatalogFirmCr,
            this.dtSections,
            this.dtCatalogNames,
            this.dtCatalog,
            this.dtProducts});
			// 
			// dtJobs
			// 
			this.dtJobs.Columns.AddRange(new System.Data.DataColumn[] {
            this.JPriceCode,
            this.JName,
            this.JRegion,
            this.JPos,
            this.JNamePos,
            this.JJobDate,
            this.JBlockBy,
            this.JMinReq,
            this.JWholeSale,
            this.JParentSynonym,
            this.JPriceFMT,
            this.JNeedRetrans,
            this.JRetranced,
            this.JParentName,
            this.JExt,
            this.JFirmCode});
			this.dtJobs.TableName = "JobsGrid";
			// 
			// JPriceCode
			// 
			this.JPriceCode.Caption = "Код прайса";
			this.JPriceCode.ColumnName = "JPriceCode";
			this.JPriceCode.DataType = typeof(long);
			this.JPriceCode.Namespace = "";
			// 
			// JName
			// 
			this.JName.Caption = "Фирмы (прайс-лист)";
			this.JName.ColumnName = "JName";
			// 
			// JRegion
			// 
			this.JRegion.Caption = "Регион";
			this.JRegion.ColumnName = "JRegion";
			// 
			// JPos
			// 
			this.JPos.Caption = "Всего";
			this.JPos.ColumnName = "JPos";
			this.JPos.DataType = typeof(long);
			// 
			// JNamePos
			// 
			this.JNamePos.Caption = "Строго";
			this.JNamePos.ColumnName = "JNamePos";
			this.JNamePos.DataType = typeof(long);
			// 
			// JJobDate
			// 
			this.JJobDate.Caption = "Дата задания";
			this.JJobDate.ColumnName = "JJobDate";
			// 
			// JBlockBy
			// 
			this.JBlockBy.Caption = "Блокировано";
			this.JBlockBy.ColumnName = "JBlockBy";
			// 
			// JMinReq
			// 
			this.JMinReq.ColumnName = "JMinReq";
			// 
			// JWholeSale
			// 
			this.JWholeSale.ColumnName = "JWholeSale";
			this.JWholeSale.DataType = typeof(long);
			this.JWholeSale.ReadOnly = true;
			// 
			// JParentSynonym
			// 
			this.JParentSynonym.ColumnName = "JParentSynonym";
			this.JParentSynonym.DataType = typeof(long);
			// 
			// JPriceFMT
			// 
			this.JPriceFMT.ColumnName = "JPriceFMT";
			// 
			// JNeedRetrans
			// 
			this.JNeedRetrans.ColumnName = "JNeedRetrans";
			this.JNeedRetrans.DataType = typeof(int);
			// 
			// JRetranced
			// 
			this.JRetranced.ColumnName = "JRetranced";
			this.JRetranced.DataType = typeof(int);
			// 
			// JParentName
			// 
			this.JParentName.ColumnName = "JParentName";
			// 
			// JExt
			// 
			this.JExt.ColumnName = "JExt";
			// 
			// JFirmCode
			// 
			this.JFirmCode.ColumnName = "JFirmCode";
			this.JFirmCode.DataType = typeof(long);
			// 
			// dtUnrecExp
			// 
			this.dtUnrecExp.Columns.AddRange(new System.Data.DataColumn[] {
            this.EUColumn1,
            this.UEColumn2,
            this.UEColumn3,
            this.UECode,
            this.UECodeCr,
            this.UEName1,
            this.UEFirmCr,
            this.UECurrency,
            this.UEBaseCost,
            this.UEUnit,
            this.UEVolume,
            this.UEQuantity,
            this.UEPeriod,
            this.UEJunk,
            this.UEStatus,
            this.UETmpProductId,
            this.UEName2,
            this.UEName3,
            this.UETmpCodeFirmCr,
            this.UEAlready,
            this.UETmpCurrency,
            this.UERowID,
            this.UEHandMade});
			this.dtUnrecExp.TableName = "UnrecExpGrid";
			// 
			// EUColumn1
			// 
			this.EUColumn1.Caption = "";
			this.EUColumn1.ColumnName = "UEColumn1";
			this.EUColumn1.DefaultValue = "";
			// 
			// UEColumn2
			// 
			this.UEColumn2.Caption = "";
			this.UEColumn2.ColumnName = "UEColumn2";
			// 
			// UEColumn3
			// 
			this.UEColumn3.Caption = "";
			this.UEColumn3.ColumnName = "UEColumn3";
			// 
			// UECode
			// 
			this.UECode.Caption = "Код";
			this.UECode.ColumnName = "UECode";
			// 
			// UECodeCr
			// 
			this.UECodeCr.Caption = "Код производителя";
			this.UECodeCr.ColumnName = "UECodeCr";
			// 
			// UEName1
			// 
			this.UEName1.Caption = "Наименование";
			this.UEName1.ColumnName = "UEName1";
			// 
			// UEFirmCr
			// 
			this.UEFirmCr.Caption = "Производитель";
			this.UEFirmCr.ColumnName = "UEFirmCr";
			// 
			// UECurrency
			// 
			this.UECurrency.Caption = "Валюта";
			this.UECurrency.ColumnName = "UECurrency";
			// 
			// UEBaseCost
			// 
			this.UEBaseCost.Caption = "Базовая цена";
			this.UEBaseCost.ColumnName = "UEBaseCost";
			this.UEBaseCost.DataType = typeof(double);
			// 
			// UEUnit
			// 
			this.UEUnit.Caption = "Ед. измерения";
			this.UEUnit.ColumnName = "UEUnit";
			// 
			// UEVolume
			// 
			this.UEVolume.Caption = "Цех. уп.";
			this.UEVolume.ColumnName = "UEVolume";
			// 
			// UEQuantity
			// 
			this.UEQuantity.Caption = "Количество";
			this.UEQuantity.ColumnName = "UEQuantity";
			// 
			// UEPeriod
			// 
			this.UEPeriod.Caption = "Срок годности";
			this.UEPeriod.ColumnName = "UEPeriod";
			// 
			// UEJunk
			// 
			this.UEJunk.Caption = "Просроченный";
			this.UEJunk.ColumnName = "UEJunk";
			this.UEJunk.DataType = typeof(byte);
			// 
			// UEStatus
			// 
			this.UEStatus.ColumnName = "UEStatus";
			this.UEStatus.DataType = typeof(int);
			// 
			// UETmpProductId
			// 
			this.UETmpProductId.ColumnName = "UETmpProductId";
			this.UETmpProductId.DataType = typeof(long);
			// 
			// UEName2
			// 
			this.UEName2.ColumnName = "UEName2";
			// 
			// UEName3
			// 
			this.UEName3.ColumnName = "UEName3";
			// 
			// UETmpCodeFirmCr
			// 
			this.UETmpCodeFirmCr.ColumnName = "UETmpCodeFirmCr";
			this.UETmpCodeFirmCr.DataType = typeof(long);
			// 
			// UEAlready
			// 
			this.UEAlready.ColumnName = "UEAlready";
			this.UEAlready.DataType = typeof(int);
			// 
			// UETmpCurrency
			// 
			this.UETmpCurrency.ColumnName = "UETmpCurrency";
			// 
			// UERowID
			// 
			this.UERowID.ColumnName = "UERowID";
			this.UERowID.DataType = typeof(long);
			// 
			// UEHandMade
			// 
			this.UEHandMade.ColumnName = "UEHandMade";
			this.UEHandMade.DataType = typeof(byte);
			// 
			// dtZero
			// 
			this.dtZero.Columns.AddRange(new System.Data.DataColumn[] {
            this.ZCode,
            this.ZCodeCr,
            this.ZName,
            this.ZFirmCr,
            this.ZCurrency,
            this.ZBaseCost,
            this.ZUnit,
            this.ZVolume,
            this.ZQuantity,
            this.ZPeriod,
            this.ZJunk});
			this.dtZero.TableName = "ZeroGrid";
			// 
			// ZCode
			// 
			this.ZCode.ColumnName = "ZCode";
			// 
			// ZCodeCr
			// 
			this.ZCodeCr.ColumnName = "ZCodeCr";
			// 
			// ZName
			// 
			this.ZName.ColumnName = "ZName";
			// 
			// ZFirmCr
			// 
			this.ZFirmCr.ColumnName = "ZFirmCr";
			// 
			// ZCurrency
			// 
			this.ZCurrency.ColumnName = "ZCurrency";
			// 
			// ZBaseCost
			// 
			this.ZBaseCost.ColumnName = "ZBaseCost";
			this.ZBaseCost.DataType = typeof(double);
			// 
			// ZUnit
			// 
			this.ZUnit.ColumnName = "ZUnit";
			// 
			// ZVolume
			// 
			this.ZVolume.ColumnName = "ZVolume";
			// 
			// ZQuantity
			// 
			this.ZQuantity.ColumnName = "ZQuantity";
			// 
			// ZPeriod
			// 
			this.ZPeriod.ColumnName = "ZPeriod";
			// 
			// ZJunk
			// 
			this.ZJunk.ColumnName = "ZJunk";
			this.ZJunk.DataType = typeof(byte);
			// 
			// dtForb
			// 
			this.dtForb.Columns.AddRange(new System.Data.DataColumn[] {
            this.FForb});
			this.dtForb.TableName = "ForbGrid";
			// 
			// FForb
			// 
			this.FForb.ColumnName = "FForb";
			// 
			// dtRegions
			// 
			this.dtRegions.Columns.AddRange(new System.Data.DataColumn[] {
            this.RRegion});
			this.dtRegions.TableName = "RegionsGrid";
			// 
			// RRegion
			// 
			this.RRegion.ColumnName = "RRegion";
			// 
			// dtCatalogFirmCr
			// 
			this.dtCatalogFirmCr.Columns.AddRange(new System.Data.DataColumn[] {
            this.CCodeFirmCr,
            this.CFirmCr});
			this.dtCatalogFirmCr.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "CCode"}, true)});
			this.dtCatalogFirmCr.PrimaryKey = new System.Data.DataColumn[] {
        this.CCodeFirmCr};
			this.dtCatalogFirmCr.TableName = "CatalogFirmCrGrid";
			// 
			// CCodeFirmCr
			// 
			this.CCodeFirmCr.AllowDBNull = false;
			this.CCodeFirmCr.ColumnName = "CCode";
			this.CCodeFirmCr.DataType = typeof(long);
			// 
			// CFirmCr
			// 
			this.CFirmCr.ColumnName = "CName";
			// 
			// dtSections
			// 
			this.dtSections.Columns.AddRange(new System.Data.DataColumn[] {
            this.SSection});
			this.dtSections.TableName = "SectionsGrid";
			// 
			// SSection
			// 
			this.SSection.ColumnName = "SSection";
			// 
			// dtCatalogNames
			// 
			this.dtCatalogNames.Columns.AddRange(new System.Data.DataColumn[] {
            this.colCatalogNameID,
            this.colCatalogNameName});
			this.dtCatalogNames.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "ID"}, false)});
			this.dtCatalogNames.TableName = "dtCatalogNames";
			// 
			// colCatalogNameID
			// 
			this.colCatalogNameID.ColumnName = "ID";
			this.colCatalogNameID.DataType = typeof(ulong);
			// 
			// colCatalogNameName
			// 
			this.colCatalogNameName.ColumnName = "Name";
			// 
			// dtCatalog
			// 
			this.dtCatalog.Columns.AddRange(new System.Data.DataColumn[] {
            this.colCatalogID,
            this.colCatalog_NameID,
            this.colCatalog_FormId,
            this.colCatalogForm,
            this.colCatalogProductsCount});
			this.dtCatalog.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.ForeignKeyConstraint("CatalogNames", "dtCatalogNames", new string[] {
                        "ID"}, new string[] {
                        "NameID"}, System.Data.AcceptRejectRule.None, System.Data.Rule.Cascade, System.Data.Rule.Cascade),
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "ID"}, false)});
			this.dtCatalog.TableName = "dtCatalog";
			// 
			// colCatalogID
			// 
			this.colCatalogID.ColumnName = "ID";
			this.colCatalogID.DataType = typeof(ulong);
			// 
			// colCatalog_NameID
			// 
			this.colCatalog_NameID.ColumnName = "NameID";
			this.colCatalog_NameID.DataType = typeof(ulong);
			// 
			// colCatalog_FormId
			// 
			this.colCatalog_FormId.ColumnName = "FormId";
			this.colCatalog_FormId.DataType = typeof(ulong);
			// 
			// colCatalogForm
			// 
			this.colCatalogForm.ColumnName = "Form";
			// 
			// colCatalogProductsCount
			// 
			this.colCatalogProductsCount.ColumnName = "ProductsCount";
			this.colCatalogProductsCount.DataType = typeof(long);
			// 
			// dtProducts
			// 
			this.dtProducts.Columns.AddRange(new System.Data.DataColumn[] {
            this.colProductId,
            this.colProductCatalogId,
            this.colProductProperties});
			this.dtProducts.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.ForeignKeyConstraint("Products", "dtCatalog", new string[] {
                        "ID"}, new string[] {
                        "CatalogId"}, System.Data.AcceptRejectRule.None, System.Data.Rule.Cascade, System.Data.Rule.Cascade)});
			this.dtProducts.TableName = "dtProducts";
			// 
			// colProductId
			// 
			this.colProductId.ColumnName = "ID";
			this.colProductId.DataType = typeof(ulong);
			// 
			// colProductCatalogId
			// 
			this.colProductCatalogId.ColumnName = "CatalogId";
			this.colProductCatalogId.DataType = typeof(ulong);
			// 
			// colProductProperties
			// 
			this.colProductProperties.ColumnName = "Properties";
			// 
			// gvCatalog
			// 
			this.gvCatalog.Appearance.HideSelectionRow.Options.UseBackColor = true;
			this.gvCatalog.ChildGridLevelName = "gvCatForm";
			this.gvCatalog.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colCName});
			this.gvCatalog.GridControl = this.CatalogGridControl;
			this.gvCatalog.Name = "gvCatalog";
			this.gvCatalog.OptionsBehavior.AllowIncrementalSearch = true;
			this.gvCatalog.OptionsBehavior.Editable = false;
			this.gvCatalog.OptionsDetail.ShowDetailTabs = false;
			this.gvCatalog.OptionsMenu.EnableColumnMenu = false;
			this.gvCatalog.OptionsMenu.EnableFooterMenu = false;
			this.gvCatalog.OptionsMenu.EnableGroupPanelMenu = false;
			this.gvCatalog.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.gvCatalog.OptionsView.ShowFilterPanel = false;
			this.gvCatalog.OptionsView.ShowGroupPanel = false;
			// 
			// colCName
			// 
			this.colCName.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.colCName.AppearanceCell.Options.UseFont = true;
			this.colCName.Caption = "Наименование";
			this.colCName.FieldName = "Name";
			this.colCName.Name = "colCName";
			this.colCName.OptionsColumn.ReadOnly = true;
			this.colCName.Visible = true;
			this.colCName.VisibleIndex = 0;
			// 
			// gvProducts
			// 
			this.gvProducts.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colProperties});
			this.gvProducts.GridControl = this.CatalogGridControl;
			this.gvProducts.Name = "gvProducts";
			this.gvProducts.OptionsBehavior.AllowIncrementalSearch = true;
			this.gvProducts.OptionsBehavior.Editable = false;
			this.gvProducts.OptionsDetail.EnableMasterViewMode = false;
			this.gvProducts.OptionsDetail.ShowDetailTabs = false;
			this.gvProducts.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.gvProducts.OptionsView.ShowDetailButtons = false;
			this.gvProducts.OptionsView.ShowFilterPanel = false;
			this.gvProducts.OptionsView.ShowGroupPanel = false;
			this.gvProducts.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gvProducts_CustomColumnDisplayText);
			// 
			// colProperties
			// 
			this.colProperties.Caption = "Свойства";
			this.colProperties.FieldName = "Properties";
			this.colProperties.Name = "colProperties";
			this.colProperties.OptionsColumn.ReadOnly = true;
			this.colProperties.Visible = true;
			this.colProperties.VisibleIndex = 0;
			// 
			// statusBarPanel1
			// 
			this.statusBarPanel1.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.statusBarPanel1.Name = "statusBarPanel1";
			this.statusBarPanel1.Width = 234;
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 731);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel1,
            this.sbpAll,
            this.sbpCurrent});
			this.statusBar1.ShowPanels = true;
			this.statusBar1.Size = new System.Drawing.Size(720, 22);
			this.statusBar1.TabIndex = 1;
			this.statusBar1.Text = "statusBar1";
			// 
			// sbpAll
			// 
			this.sbpAll.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.sbpAll.Name = "sbpAll";
			this.sbpAll.Width = 234;
			// 
			// sbpCurrent
			// 
			this.sbpCurrent.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.sbpCurrent.Name = "sbpCurrent";
			this.sbpCurrent.Width = 234;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.tcMain);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(0, 0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(720, 731);
			this.panel3.TabIndex = 2;
			// 
			// tcMain
			// 
			this.tcMain.Controls.Add(this.tpJobs);
			this.tcMain.Controls.Add(this.tpUnrecExp);
			this.tcMain.Controls.Add(this.tpZero);
			this.tcMain.Controls.Add(this.tpForb);
			this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcMain.Location = new System.Drawing.Point(0, 0);
			this.tcMain.Name = "tcMain";
			this.tcMain.SelectedIndex = 0;
			this.tcMain.Size = new System.Drawing.Size(720, 731);
			this.tcMain.TabIndex = 0;
			this.tcMain.SelectedIndexChanged += new System.EventHandler(this.tcMain_SelectedIndexChanged);
			// 
			// tpJobs
			// 
			this.tpJobs.Controls.Add(this.pnlCenter1);
			this.tpJobs.Location = new System.Drawing.Point(4, 22);
			this.tpJobs.Name = "tpJobs";
			this.tpJobs.Size = new System.Drawing.Size(712, 705);
			this.tpJobs.TabIndex = 0;
			this.tpJobs.Text = "Задания";
			// 
			// pnlCenter1
			// 
			this.pnlCenter1.Controls.Add(this.pnlTop1);
			this.pnlCenter1.Controls.Add(this.pnlWithButton1);
			this.pnlCenter1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlCenter1.Location = new System.Drawing.Point(0, 0);
			this.pnlCenter1.Name = "pnlCenter1";
			this.pnlCenter1.Size = new System.Drawing.Size(712, 705);
			this.pnlCenter1.TabIndex = 5;
			// 
			// pnlTop1
			// 
			this.pnlTop1.Controls.Add(this.JobsGridControl);
			this.pnlTop1.Controls.Add(this.groupBox1);
			this.pnlTop1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlTop1.Location = new System.Drawing.Point(0, 0);
			this.pnlTop1.Name = "pnlTop1";
			this.pnlTop1.Size = new System.Drawing.Size(712, 681);
			this.pnlTop1.TabIndex = 1;
			// 
			// JobsGridControl
			// 
			this.JobsGridControl.DataMember = "JobsGrid";
			this.JobsGridControl.DataSource = this.dsMain;
			this.JobsGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			// 
			// 
			// 
			this.JobsGridControl.EmbeddedNavigator.Name = "";
			this.JobsGridControl.Location = new System.Drawing.Point(0, 0);
			this.JobsGridControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
			this.JobsGridControl.LookAndFeel.UseDefaultLookAndFeel = false;
			this.JobsGridControl.LookAndFeel.UseWindowsXPTheme = true;
			this.JobsGridControl.MainView = this.gvJobs;
			this.JobsGridControl.Name = "JobsGridControl";
			this.JobsGridControl.Size = new System.Drawing.Size(592, 681);
			this.JobsGridControl.TabIndex = 4;
			this.JobsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvJobs});
			this.JobsGridControl.DoubleClick += new System.EventHandler(this.JobsGridControl_DoubleClick);
			this.JobsGridControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JobsGridControl_KeyDown);
			// 
			// gvJobs
			// 
			this.gvJobs.Appearance.FocusedCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.gvJobs.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colJName,
            this.colJParentName,
            this.colJWholeSale,
            this.colJRegion,
            this.colJNeedRetrans,
            this.colJRetranced,
            this.colJPos,
            this.colJNamePos,
            this.colJJobDate,
            this.colJBlockBy});
			this.gvJobs.GridControl = this.JobsGridControl;
			this.gvJobs.Images = this.imageList1;
			this.gvJobs.IndicatorWidth = 16;
			this.gvJobs.Name = "gvJobs";
			this.gvJobs.OptionsBehavior.AllowIncrementalSearch = true;
			this.gvJobs.OptionsMenu.EnableColumnMenu = false;
			this.gvJobs.OptionsMenu.EnableFooterMenu = false;
			this.gvJobs.OptionsMenu.EnableGroupPanelMenu = false;
			this.gvJobs.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.gvJobs.OptionsView.ShowGroupPanel = false;
			this.gvJobs.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvJobs_RowStyle);
			this.gvJobs.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvJobs_CustomDrawRowIndicator);
			this.gvJobs.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gvJobs_CustomColumnDisplayText);
			// 
			// colJName
			// 
			this.colJName.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.colJName.AppearanceCell.Options.UseFont = true;
			this.colJName.Caption = "Фирма (прайс-лист)";
			this.colJName.FieldName = "JName";
			this.colJName.Name = "colJName";
			this.colJName.OptionsColumn.AllowEdit = false;
			this.colJName.OptionsColumn.ReadOnly = true;
			this.colJName.Visible = true;
			this.colJName.VisibleIndex = 0;
			// 
			// colJParentName
			// 
			this.colJParentName.Caption = "Родитель";
			this.colJParentName.FieldName = "JParentName";
			this.colJParentName.Name = "colJParentName";
			this.colJParentName.OptionsColumn.AllowEdit = false;
			this.colJParentName.OptionsColumn.AllowFocus = false;
			this.colJParentName.OptionsColumn.AllowIncrementalSearch = false;
			this.colJParentName.OptionsColumn.ReadOnly = true;
			this.colJParentName.Visible = true;
			this.colJParentName.VisibleIndex = 1;
			// 
			// colJWholeSale
			// 
			this.colJWholeSale.AppearanceCell.Options.UseTextOptions = true;
			this.colJWholeSale.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.colJWholeSale.Caption = "Сегмент";
			this.colJWholeSale.FieldName = "JWholeSale";
			this.colJWholeSale.Name = "colJWholeSale";
			this.colJWholeSale.OptionsColumn.AllowEdit = false;
			this.colJWholeSale.OptionsColumn.AllowFocus = false;
			this.colJWholeSale.OptionsColumn.AllowIncrementalSearch = false;
			this.colJWholeSale.OptionsColumn.ReadOnly = true;
			this.colJWholeSale.Visible = true;
			this.colJWholeSale.VisibleIndex = 2;
			// 
			// colJRegion
			// 
			this.colJRegion.Caption = "Регион";
			this.colJRegion.FieldName = "JRegion";
			this.colJRegion.Name = "colJRegion";
			this.colJRegion.OptionsColumn.AllowEdit = false;
			this.colJRegion.OptionsColumn.AllowFocus = false;
			this.colJRegion.OptionsColumn.AllowIncrementalSearch = false;
			this.colJRegion.OptionsColumn.ReadOnly = true;
			this.colJRegion.Visible = true;
			this.colJRegion.VisibleIndex = 3;
			// 
			// colJNeedRetrans
			// 
			this.colJNeedRetrans.Caption = "Требуется формализация";
			this.colJNeedRetrans.FieldName = "JNeedRetrans";
			this.colJNeedRetrans.Name = "colJNeedRetrans";
			this.colJNeedRetrans.OptionsColumn.AllowEdit = false;
			this.colJNeedRetrans.OptionsColumn.AllowFocus = false;
			this.colJNeedRetrans.OptionsColumn.AllowIncrementalSearch = false;
			this.colJNeedRetrans.OptionsColumn.ReadOnly = true;
			this.colJNeedRetrans.ToolTip = "Требуется формализация";
			this.colJNeedRetrans.Visible = true;
			this.colJNeedRetrans.VisibleIndex = 4;
			// 
			// colJRetranced
			// 
			this.colJRetranced.Caption = "В очереди";
			this.colJRetranced.FieldName = "JRetranced";
			this.colJRetranced.Name = "colJRetranced";
			this.colJRetranced.OptionsColumn.AllowEdit = false;
			this.colJRetranced.OptionsColumn.AllowFocus = false;
			this.colJRetranced.OptionsColumn.AllowIncrementalSearch = false;
			this.colJRetranced.OptionsColumn.ReadOnly = true;
			this.colJRetranced.ToolTip = "В очереди";
			this.colJRetranced.Visible = true;
			this.colJRetranced.VisibleIndex = 5;
			// 
			// colJPos
			// 
			this.colJPos.Caption = "Всего";
			this.colJPos.FieldName = "JPos";
			this.colJPos.Name = "colJPos";
			this.colJPos.OptionsColumn.AllowEdit = false;
			this.colJPos.OptionsColumn.AllowFocus = false;
			this.colJPos.OptionsColumn.AllowIncrementalSearch = false;
			this.colJPos.OptionsColumn.ReadOnly = true;
			this.colJPos.Visible = true;
			this.colJPos.VisibleIndex = 6;
			// 
			// colJNamePos
			// 
			this.colJNamePos.Caption = "Строго";
			this.colJNamePos.FieldName = "JNamePos";
			this.colJNamePos.Name = "colJNamePos";
			this.colJNamePos.OptionsColumn.AllowEdit = false;
			this.colJNamePos.OptionsColumn.AllowFocus = false;
			this.colJNamePos.OptionsColumn.AllowIncrementalSearch = false;
			this.colJNamePos.OptionsColumn.ReadOnly = true;
			this.colJNamePos.Visible = true;
			this.colJNamePos.VisibleIndex = 7;
			// 
			// colJJobDate
			// 
			this.colJJobDate.Caption = "Дата задания";
			this.colJJobDate.FieldName = "JJobDate";
			this.colJJobDate.Name = "colJJobDate";
			this.colJJobDate.OptionsColumn.AllowEdit = false;
			this.colJJobDate.OptionsColumn.AllowFocus = false;
			this.colJJobDate.OptionsColumn.AllowIncrementalSearch = false;
			this.colJJobDate.OptionsColumn.ReadOnly = true;
			this.colJJobDate.Visible = true;
			this.colJJobDate.VisibleIndex = 8;
			// 
			// colJBlockBy
			// 
			this.colJBlockBy.Caption = "Блокировано";
			this.colJBlockBy.FieldName = "JBlockBy";
			this.colJBlockBy.Name = "colJBlockBy";
			this.colJBlockBy.OptionsColumn.AllowEdit = false;
			this.colJBlockBy.OptionsColumn.AllowFocus = false;
			this.colJBlockBy.OptionsColumn.AllowIncrementalSearch = false;
			this.colJBlockBy.OptionsColumn.ReadOnly = true;
			this.colJBlockBy.Visible = true;
			this.colJBlockBy.VisibleIndex = 9;
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Silver;
			this.imageList1.Images.SetKeyName(0, "");
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnJobs50);
			this.groupBox1.Controls.Add(this.btnJobsNamePos);
			this.groupBox1.Controls.Add(this.btnJobsBlock);
			this.groupBox1.Controls.Add(this.lbJobs50Text);
			this.groupBox1.Controls.Add(this.lbJobsNamePosText);
			this.groupBox1.Controls.Add(this.lbJobsBlockText);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
			this.groupBox1.Location = new System.Drawing.Point(592, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(120, 681);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Легенда";
			// 
			// btnJobs50
			// 
			this.btnJobs50.BackColor = System.Drawing.Color.NavajoWhite;
			this.btnJobs50.Location = new System.Drawing.Point(8, 160);
			this.btnJobs50.Name = "btnJobs50";
			this.btnJobs50.Size = new System.Drawing.Size(32, 16);
			this.btnJobs50.TabIndex = 19;
			this.btnJobs50.UseVisualStyleBackColor = false;
			this.btnJobs50.Click += new System.EventHandler(this.lbColorChange);
			// 
			// btnJobsNamePos
			// 
			this.btnJobsNamePos.BackColor = System.Drawing.Color.Coral;
			this.btnJobsNamePos.Location = new System.Drawing.Point(8, 88);
			this.btnJobsNamePos.Name = "btnJobsNamePos";
			this.btnJobsNamePos.Size = new System.Drawing.Size(32, 16);
			this.btnJobsNamePos.TabIndex = 18;
			this.btnJobsNamePos.UseVisualStyleBackColor = false;
			this.btnJobsNamePos.Click += new System.EventHandler(this.lbColorChange);
			// 
			// btnJobsBlock
			// 
			this.btnJobsBlock.BackColor = System.Drawing.Color.OrangeRed;
			this.btnJobsBlock.Location = new System.Drawing.Point(8, 24);
			this.btnJobsBlock.Name = "btnJobsBlock";
			this.btnJobsBlock.Size = new System.Drawing.Size(32, 16);
			this.btnJobsBlock.TabIndex = 17;
			this.btnJobsBlock.UseVisualStyleBackColor = false;
			this.btnJobsBlock.Click += new System.EventHandler(this.lbColorChange);
			// 
			// lbJobs50Text
			// 
			this.lbJobs50Text.Location = new System.Drawing.Point(8, 176);
			this.lbJobs50Text.Name = "lbJobs50Text";
			this.lbJobs50Text.Size = new System.Drawing.Size(104, 56);
			this.lbJobs50Text.TabIndex = 6;
			this.lbJobs50Text.Text = "Процент позиций,  нераспознанных по наименованию меньше 50";
			// 
			// lbJobsNamePosText
			// 
			this.lbJobsNamePosText.Location = new System.Drawing.Point(8, 104);
			this.lbJobsNamePosText.Name = "lbJobsNamePosText";
			this.lbJobsNamePosText.Size = new System.Drawing.Size(104, 40);
			this.lbJobsNamePosText.TabIndex = 5;
			this.lbJobsNamePosText.Text = "Нет позиций нерапознанных по наименованию";
			// 
			// lbJobsBlockText
			// 
			this.lbJobsBlockText.Location = new System.Drawing.Point(8, 40);
			this.lbJobsBlockText.Name = "lbJobsBlockText";
			this.lbJobsBlockText.Size = new System.Drawing.Size(80, 40);
			this.lbJobsBlockText.TabIndex = 4;
			this.lbJobsBlockText.Text = "Прайс-лист заблокирован оператором";
			// 
			// pnlWithButton1
			// 
			this.pnlWithButton1.Controls.Add(this.btnDelJob);
			this.pnlWithButton1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlWithButton1.Location = new System.Drawing.Point(0, 681);
			this.pnlWithButton1.Name = "pnlWithButton1";
			this.pnlWithButton1.Size = new System.Drawing.Size(712, 24);
			this.pnlWithButton1.TabIndex = 0;
			// 
			// btnDelJob
			// 
			this.btnDelJob.BackColor = System.Drawing.SystemColors.Control;
			this.btnDelJob.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnDelJob.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnDelJob.ImageIndex = 4;
			this.btnDelJob.ImageList = this.imageList2;
			this.btnDelJob.Location = new System.Drawing.Point(528, 0);
			this.btnDelJob.Name = "btnDelJob";
			this.btnDelJob.Size = new System.Drawing.Size(184, 24);
			this.btnDelJob.TabIndex = 6;
			this.btnDelJob.Text = "Удалить ЗАДАНИЕ";
			this.btnDelJob.UseVisualStyleBackColor = false;
			this.btnDelJob.Click += new System.EventHandler(this.btnDelJob_Click);
			// 
			// imageList2
			// 
			this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
			this.imageList2.TransparentColor = System.Drawing.SystemColors.Control;
			this.imageList2.Images.SetKeyName(0, "");
			this.imageList2.Images.SetKeyName(1, "");
			this.imageList2.Images.SetKeyName(2, "");
			this.imageList2.Images.SetKeyName(3, "");
			this.imageList2.Images.SetKeyName(4, "");
			// 
			// tpUnrecExp
			// 
			this.tpUnrecExp.ContextMenu = this.cmUnrecExp;
			this.tpUnrecExp.Controls.Add(this.pnlBottom2);
			this.tpUnrecExp.Controls.Add(this.pnlCenter2);
			this.tpUnrecExp.Controls.Add(this.pnlTop2);
			this.tpUnrecExp.Location = new System.Drawing.Point(4, 22);
			this.tpUnrecExp.Name = "tpUnrecExp";
			this.tpUnrecExp.Size = new System.Drawing.Size(712, 705);
			this.tpUnrecExp.TabIndex = 1;
			this.tpUnrecExp.Text = "Нераспознанные";
			this.tpUnrecExp.Visible = false;
			// 
			// cmUnrecExp
			// 
			this.cmUnrecExp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miSendAboutNames,
            this.miSendAboutFirmCr});
			// 
			// miSendAboutNames
			// 
			this.miSendAboutNames.Index = 0;
			this.miSendAboutNames.Text = "Отправить письмо о наименованиях";
			this.miSendAboutNames.Click += new System.EventHandler(this.miSendAboutNames_Click);
			// 
			// miSendAboutFirmCr
			// 
			this.miSendAboutFirmCr.Index = 1;
			this.miSendAboutFirmCr.Text = "Отправить письмо о производителях";
			this.miSendAboutFirmCr.Click += new System.EventHandler(this.miSendAboutFirmCr_Click);
			// 
			// pnlBottom2
			// 
			this.pnlBottom2.Controls.Add(this.UnrecExpGridControl);
			this.pnlBottom2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlBottom2.Location = new System.Drawing.Point(0, 280);
			this.pnlBottom2.Name = "pnlBottom2";
			this.pnlBottom2.Size = new System.Drawing.Size(712, 425);
			this.pnlBottom2.TabIndex = 7;
			// 
			// UnrecExpGridControl
			// 
			this.UnrecExpGridControl.DataSource = this.dtUnrecExp;
			this.UnrecExpGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			// 
			// 
			// 
			this.UnrecExpGridControl.EmbeddedNavigator.Name = "";
			this.UnrecExpGridControl.Location = new System.Drawing.Point(0, 0);
			this.UnrecExpGridControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
			this.UnrecExpGridControl.LookAndFeel.UseDefaultLookAndFeel = false;
			this.UnrecExpGridControl.LookAndFeel.UseWindowsXPTheme = true;
			this.UnrecExpGridControl.MainView = this.gvUnrecExp;
			this.UnrecExpGridControl.Name = "UnrecExpGridControl";
			this.UnrecExpGridControl.Size = new System.Drawing.Size(712, 425);
			this.UnrecExpGridControl.TabIndex = 1;
			this.UnrecExpGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvUnrecExp});
			this.UnrecExpGridControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UnrecExpGridControl_KeyDown);
			this.UnrecExpGridControl.Click += new System.EventHandler(this.UnrecExpGridControl_Click);
			// 
			// gvUnrecExp
			// 
			this.gvUnrecExp.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colUEColumn1,
            this.colUEColumn2,
            this.colUEColumn3,
            this.colUECode,
            this.colUECodeCr,
            this.colUEName1,
            this.colUEFirmCr,
            this.colUECurrency,
            this.colUEBaseCost,
            this.colUEUnit,
            this.colUEVolume,
            this.colUEQuantity,
            this.colUEPeriod,
            this.colUEJunk,
            this.colUEStatus,
            this.colUEAlready});
			this.gvUnrecExp.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
			this.gvUnrecExp.GridControl = this.UnrecExpGridControl;
			this.gvUnrecExp.Name = "gvUnrecExp";
			this.gvUnrecExp.OptionsCustomization.AllowSort = false;
			this.gvUnrecExp.OptionsDetail.EnableMasterViewMode = false;
			this.gvUnrecExp.OptionsMenu.EnableColumnMenu = false;
			this.gvUnrecExp.OptionsMenu.EnableFooterMenu = false;
			this.gvUnrecExp.OptionsMenu.EnableGroupPanelMenu = false;
			this.gvUnrecExp.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.gvUnrecExp.OptionsView.ShowGroupPanel = false;
			this.gvUnrecExp.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvUnrecExp_RowCellStyle);
			this.gvUnrecExp.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gvUnrecExp_CustomDrawCell);
			this.gvUnrecExp.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvUnrecExp_FocusedRowChanged);
			this.gvUnrecExp.CustomColumnSort += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(this.gvUnrecExp_CustomColumnSort);
			this.gvUnrecExp.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gvUnrecExp_CustomColumnDisplayText);
			// 
			// colUEColumn1
			// 
			this.colUEColumn1.Caption = "Name";
			this.colUEColumn1.FieldName = "UEColumn1";
			this.colUEColumn1.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
			this.colUEColumn1.Name = "colUEColumn1";
			this.colUEColumn1.OptionsColumn.AllowEdit = false;
			this.colUEColumn1.OptionsColumn.AllowSize = false;
			this.colUEColumn1.OptionsColumn.FixedWidth = true;
			this.colUEColumn1.ToolTip = "Признак того, что позиция распознана по наименованию";
			this.colUEColumn1.Visible = true;
			this.colUEColumn1.VisibleIndex = 0;
			this.colUEColumn1.Width = 20;
			// 
			// colUEColumn2
			// 
			this.colUEColumn2.Caption = "FirmCr";
			this.colUEColumn2.FieldName = "UEColumn2";
			this.colUEColumn2.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
			this.colUEColumn2.Name = "colUEColumn2";
			this.colUEColumn2.OptionsColumn.AllowEdit = false;
			this.colUEColumn2.OptionsColumn.AllowSize = false;
			this.colUEColumn2.OptionsColumn.FixedWidth = true;
			this.colUEColumn2.ToolTip = "Признак того, что позиция распознана по производителю";
			this.colUEColumn2.Visible = true;
			this.colUEColumn2.VisibleIndex = 1;
			this.colUEColumn2.Width = 20;
			// 
			// colUEColumn3
			// 
			this.colUEColumn3.Caption = "Curr";
			this.colUEColumn3.FieldName = "UEColumn3";
			this.colUEColumn3.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
			this.colUEColumn3.Name = "colUEColumn3";
			this.colUEColumn3.OptionsColumn.AllowEdit = false;
			this.colUEColumn3.OptionsColumn.AllowSize = false;
			this.colUEColumn3.OptionsColumn.FixedWidth = true;
			this.colUEColumn3.ToolTip = "Признак того, что позиция распознана по валюте";
			this.colUEColumn3.Visible = true;
			this.colUEColumn3.VisibleIndex = 2;
			this.colUEColumn3.Width = 20;
			// 
			// colUECode
			// 
			this.colUECode.Caption = "Код";
			this.colUECode.FieldName = "UECode";
			this.colUECode.Name = "colUECode";
			this.colUECode.OptionsColumn.AllowEdit = false;
			this.colUECode.Visible = true;
			this.colUECode.VisibleIndex = 3;
			this.colUECode.Width = 55;
			// 
			// colUECodeCr
			// 
			this.colUECodeCr.Caption = "Код производителя";
			this.colUECodeCr.FieldName = "UECodeCr";
			this.colUECodeCr.Name = "colUECodeCr";
			this.colUECodeCr.OptionsColumn.AllowEdit = false;
			this.colUECodeCr.Visible = true;
			this.colUECodeCr.VisibleIndex = 4;
			this.colUECodeCr.Width = 55;
			// 
			// colUEName1
			// 
			this.colUEName1.Caption = "Наименование";
			this.colUEName1.FieldName = "UEName1";
			this.colUEName1.Name = "colUEName1";
			this.colUEName1.OptionsColumn.AllowEdit = false;
			this.colUEName1.Visible = true;
			this.colUEName1.VisibleIndex = 5;
			this.colUEName1.Width = 55;
			// 
			// colUEFirmCr
			// 
			this.colUEFirmCr.Caption = "Производитель";
			this.colUEFirmCr.FieldName = "UEFirmCr";
			this.colUEFirmCr.Name = "colUEFirmCr";
			this.colUEFirmCr.OptionsColumn.AllowEdit = false;
			this.colUEFirmCr.Visible = true;
			this.colUEFirmCr.VisibleIndex = 6;
			this.colUEFirmCr.Width = 55;
			// 
			// colUECurrency
			// 
			this.colUECurrency.Caption = "Валюта";
			this.colUECurrency.FieldName = "UECurrency";
			this.colUECurrency.Name = "colUECurrency";
			this.colUECurrency.OptionsColumn.AllowEdit = false;
			this.colUECurrency.Visible = true;
			this.colUECurrency.VisibleIndex = 7;
			this.colUECurrency.Width = 55;
			// 
			// colUEBaseCost
			// 
			this.colUEBaseCost.Caption = "Базовая цена";
			this.colUEBaseCost.FieldName = "UEBaseCost";
			this.colUEBaseCost.Name = "colUEBaseCost";
			this.colUEBaseCost.OptionsColumn.AllowEdit = false;
			this.colUEBaseCost.Visible = true;
			this.colUEBaseCost.VisibleIndex = 8;
			this.colUEBaseCost.Width = 55;
			// 
			// colUEUnit
			// 
			this.colUEUnit.Caption = "Ед. измерения";
			this.colUEUnit.FieldName = "UEUnit";
			this.colUEUnit.Name = "colUEUnit";
			this.colUEUnit.OptionsColumn.AllowEdit = false;
			this.colUEUnit.Visible = true;
			this.colUEUnit.VisibleIndex = 9;
			this.colUEUnit.Width = 55;
			// 
			// colUEVolume
			// 
			this.colUEVolume.Caption = "Цех. уп.";
			this.colUEVolume.FieldName = "UEVolume";
			this.colUEVolume.Name = "colUEVolume";
			this.colUEVolume.OptionsColumn.AllowEdit = false;
			this.colUEVolume.Visible = true;
			this.colUEVolume.VisibleIndex = 10;
			this.colUEVolume.Width = 55;
			// 
			// colUEQuantity
			// 
			this.colUEQuantity.Caption = "Количество";
			this.colUEQuantity.FieldName = "UEQuantity";
			this.colUEQuantity.Name = "colUEQuantity";
			this.colUEQuantity.OptionsColumn.AllowEdit = false;
			this.colUEQuantity.Visible = true;
			this.colUEQuantity.VisibleIndex = 11;
			this.colUEQuantity.Width = 55;
			// 
			// colUEPeriod
			// 
			this.colUEPeriod.Caption = "Срок годности";
			this.colUEPeriod.FieldName = "UEPeriod";
			this.colUEPeriod.Name = "colUEPeriod";
			this.colUEPeriod.OptionsColumn.AllowEdit = false;
			this.colUEPeriod.Visible = true;
			this.colUEPeriod.VisibleIndex = 12;
			this.colUEPeriod.Width = 55;
			// 
			// colUEJunk
			// 
			this.colUEJunk.Caption = "Просроченный";
			this.colUEJunk.FieldName = "UEJunk";
			this.colUEJunk.Name = "colUEJunk";
			this.colUEJunk.OptionsColumn.AllowEdit = false;
			this.colUEJunk.Visible = true;
			this.colUEJunk.VisibleIndex = 13;
			this.colUEJunk.Width = 55;
			// 
			// colUEStatus
			// 
			this.colUEStatus.Caption = "UEStatus";
			this.colUEStatus.FieldName = "UEStatus";
			this.colUEStatus.Name = "colUEStatus";
			// 
			// colUEAlready
			// 
			this.colUEAlready.Caption = "UEAlready";
			this.colUEAlready.FieldName = "UEAlready";
			this.colUEAlready.Name = "colUEAlready";
			this.colUEAlready.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
			// 
			// pnlCenter2
			// 
			this.pnlCenter2.Controls.Add(this.pnlLeft2);
			this.pnlCenter2.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlCenter2.Location = new System.Drawing.Point(0, 24);
			this.pnlCenter2.Name = "pnlCenter2";
			this.pnlCenter2.Size = new System.Drawing.Size(712, 256);
			this.pnlCenter2.TabIndex = 6;
			// 
			// pnlLeft2
			// 
			this.pnlLeft2.Controls.Add(this.grpBoxCatalog2);
			this.pnlLeft2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlLeft2.Location = new System.Drawing.Point(0, 0);
			this.pnlLeft2.Name = "pnlLeft2";
			this.pnlLeft2.Size = new System.Drawing.Size(712, 256);
			this.pnlLeft2.TabIndex = 1;
			// 
			// grpBoxCatalog2
			// 
			this.grpBoxCatalog2.Controls.Add(this.CatalogGridControl);
			this.grpBoxCatalog2.Controls.Add(this.gcFirmCr);
			this.grpBoxCatalog2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grpBoxCatalog2.Location = new System.Drawing.Point(0, 0);
			this.grpBoxCatalog2.Name = "grpBoxCatalog2";
			this.grpBoxCatalog2.Size = new System.Drawing.Size(712, 256);
			this.grpBoxCatalog2.TabIndex = 0;
			this.grpBoxCatalog2.TabStop = false;
			this.grpBoxCatalog2.Text = "Каталог";
			// 
			// gcFirmCr
			// 
			this.gcFirmCr.DataMember = "CatalogFirmCrGrid";
			this.gcFirmCr.DataSource = this.dsMain;
			this.gcFirmCr.Dock = System.Windows.Forms.DockStyle.Fill;
			// 
			// 
			// 
			this.gcFirmCr.EmbeddedNavigator.Name = "";
			this.gcFirmCr.Enabled = false;
			this.gcFirmCr.Location = new System.Drawing.Point(3, 16);
			this.gcFirmCr.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
			this.gcFirmCr.LookAndFeel.UseDefaultLookAndFeel = false;
			this.gcFirmCr.LookAndFeel.UseWindowsXPTheme = true;
			this.gcFirmCr.MainView = this.gvFirmCr;
			this.gcFirmCr.Name = "gcFirmCr";
			this.gcFirmCr.Size = new System.Drawing.Size(706, 237);
			this.gcFirmCr.TabIndex = 1;
			this.gcFirmCr.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvFirmCr,
            this.gridView1,
            this.gridView2});
			this.gcFirmCr.Visible = false;
			this.gcFirmCr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gcFirmCr_KeyDown);
			// 
			// gvFirmCr
			// 
			this.gvFirmCr.Appearance.HideSelectionRow.Options.UseBackColor = true;
			this.gvFirmCr.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colFirmCrName});
			this.gvFirmCr.GridControl = this.gcFirmCr;
			this.gvFirmCr.Name = "gvFirmCr";
			this.gvFirmCr.OptionsBehavior.AllowIncrementalSearch = true;
			this.gvFirmCr.OptionsDetail.ShowDetailTabs = false;
			this.gvFirmCr.OptionsMenu.EnableColumnMenu = false;
			this.gvFirmCr.OptionsMenu.EnableFooterMenu = false;
			this.gvFirmCr.OptionsMenu.EnableGroupPanelMenu = false;
			this.gvFirmCr.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.gvFirmCr.OptionsView.ShowFilterPanel = false;
			this.gvFirmCr.OptionsView.ShowGroupPanel = false;
			// 
			// colFirmCrName
			// 
			this.colFirmCrName.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.colFirmCrName.AppearanceCell.Options.UseFont = true;
			this.colFirmCrName.Caption = "Наименование";
			this.colFirmCrName.FieldName = "CName";
			this.colFirmCrName.Name = "colFirmCrName";
			this.colFirmCrName.OptionsColumn.AllowEdit = false;
			this.colFirmCrName.OptionsColumn.ReadOnly = true;
			this.colFirmCrName.Visible = true;
			this.colFirmCrName.VisibleIndex = 0;
			// 
			// gridView1
			// 
			this.gridView1.Appearance.HideSelectionRow.Options.UseBackColor = true;
			this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1});
			this.gridView1.GridControl = this.gcFirmCr;
			this.gridView1.Name = "gridView1";
			this.gridView1.OptionsBehavior.AllowIncrementalSearch = true;
			this.gridView1.OptionsBehavior.Editable = false;
			this.gridView1.OptionsDetail.ShowDetailTabs = false;
			this.gridView1.OptionsFilter.AllowColumnMRUFilterList = false;
			this.gridView1.OptionsFilter.AllowMRUFilterList = false;
			this.gridView1.OptionsMenu.EnableColumnMenu = false;
			this.gridView1.OptionsMenu.EnableFooterMenu = false;
			this.gridView1.OptionsMenu.EnableGroupPanelMenu = false;
			this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.gridView1.OptionsView.ShowDetailButtons = false;
			this.gridView1.OptionsView.ShowFilterPanel = false;
			this.gridView1.OptionsView.ShowGroupPanel = false;
			// 
			// gridColumn1
			// 
			this.gridColumn1.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.gridColumn1.AppearanceCell.Options.UseFont = true;
			this.gridColumn1.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.gridColumn1.AppearanceHeader.Options.UseFont = true;
			this.gridColumn1.Caption = "Форма выпуска";
			this.gridColumn1.FieldName = "Form";
			this.gridColumn1.Name = "gridColumn1";
			this.gridColumn1.OptionsColumn.ReadOnly = true;
			this.gridColumn1.Visible = true;
			this.gridColumn1.VisibleIndex = 0;
			// 
			// gridView2
			// 
			this.gridView2.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn2});
			this.gridView2.GridControl = this.gcFirmCr;
			this.gridView2.Name = "gridView2";
			this.gridView2.OptionsBehavior.AllowIncrementalSearch = true;
			this.gridView2.OptionsBehavior.Editable = false;
			this.gridView2.OptionsDetail.EnableMasterViewMode = false;
			this.gridView2.OptionsDetail.ShowDetailTabs = false;
			this.gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.gridView2.OptionsView.ShowDetailButtons = false;
			this.gridView2.OptionsView.ShowFilterPanel = false;
			this.gridView2.OptionsView.ShowGroupPanel = false;
			// 
			// gridColumn2
			// 
			this.gridColumn2.Caption = "Свойства";
			this.gridColumn2.FieldName = "Properties";
			this.gridColumn2.Name = "gridColumn2";
			this.gridColumn2.OptionsColumn.ReadOnly = true;
			this.gridColumn2.Visible = true;
			this.gridColumn2.VisibleIndex = 0;
			// 
			// pnlTop2
			// 
			this.pnlTop2.Controls.Add(this.BigNameLabel2);
			this.pnlTop2.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlTop2.Location = new System.Drawing.Point(0, 0);
			this.pnlTop2.Name = "pnlTop2";
			this.pnlTop2.Size = new System.Drawing.Size(712, 24);
			this.pnlTop2.TabIndex = 0;
			// 
			// BigNameLabel2
			// 
			this.BigNameLabel2.BackColor = System.Drawing.SystemColors.Info;
			this.BigNameLabel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.BigNameLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.BigNameLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.BigNameLabel2.Location = new System.Drawing.Point(0, 0);
			this.BigNameLabel2.Name = "BigNameLabel2";
			this.BigNameLabel2.Size = new System.Drawing.Size(712, 24);
			this.BigNameLabel2.TabIndex = 0;
			this.BigNameLabel2.Text = "BigNameLabel";
			this.BigNameLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tpZero
			// 
			this.tpZero.Controls.Add(this.ZeroGridControl);
			this.tpZero.Location = new System.Drawing.Point(4, 22);
			this.tpZero.Name = "tpZero";
			this.tpZero.Size = new System.Drawing.Size(712, 705);
			this.tpZero.TabIndex = 2;
			this.tpZero.Text = "Цена 0";
			this.tpZero.Visible = false;
			// 
			// ZeroGridControl
			// 
			this.ZeroGridControl.DataSource = this.dtZero;
			this.ZeroGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			// 
			// 
			// 
			this.ZeroGridControl.EmbeddedNavigator.Name = "";
			this.ZeroGridControl.Location = new System.Drawing.Point(0, 0);
			this.ZeroGridControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
			this.ZeroGridControl.LookAndFeel.UseDefaultLookAndFeel = false;
			this.ZeroGridControl.LookAndFeel.UseWindowsXPTheme = true;
			this.ZeroGridControl.MainView = this.gvZero;
			this.ZeroGridControl.Name = "ZeroGridControl";
			this.ZeroGridControl.Size = new System.Drawing.Size(712, 705);
			this.ZeroGridControl.TabIndex = 0;
			this.ZeroGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvZero});
			// 
			// gvZero
			// 
			this.gvZero.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colZCode,
            this.colZCodeCr,
            this.colZName,
            this.colZFirmCr,
            this.colZCurrency,
            this.colZUnit,
            this.colZVolume,
            this.colZQuantity,
            this.colZPeriod});
			this.gvZero.GridControl = this.ZeroGridControl;
			this.gvZero.Name = "gvZero";
			this.gvZero.OptionsMenu.EnableColumnMenu = false;
			this.gvZero.OptionsMenu.EnableFooterMenu = false;
			this.gvZero.OptionsMenu.EnableGroupPanelMenu = false;
			this.gvZero.OptionsView.ShowGroupPanel = false;
			// 
			// colZCode
			// 
			this.colZCode.Caption = "Код";
			this.colZCode.FieldName = "ZCode";
			this.colZCode.Name = "colZCode";
			this.colZCode.OptionsColumn.AllowEdit = false;
			this.colZCode.Visible = true;
			this.colZCode.VisibleIndex = 0;
			// 
			// colZCodeCr
			// 
			this.colZCodeCr.Caption = "Код производителя";
			this.colZCodeCr.FieldName = "ZCodeCr";
			this.colZCodeCr.Name = "colZCodeCr";
			this.colZCodeCr.OptionsColumn.AllowEdit = false;
			this.colZCodeCr.Visible = true;
			this.colZCodeCr.VisibleIndex = 1;
			// 
			// colZName
			// 
			this.colZName.Caption = "Наименование";
			this.colZName.FieldName = "ZName";
			this.colZName.Name = "colZName";
			this.colZName.OptionsColumn.AllowEdit = false;
			this.colZName.Visible = true;
			this.colZName.VisibleIndex = 2;
			// 
			// colZFirmCr
			// 
			this.colZFirmCr.Caption = "Производитель";
			this.colZFirmCr.FieldName = "ZFirmCr";
			this.colZFirmCr.Name = "colZFirmCr";
			this.colZFirmCr.OptionsColumn.AllowEdit = false;
			this.colZFirmCr.Visible = true;
			this.colZFirmCr.VisibleIndex = 3;
			// 
			// colZCurrency
			// 
			this.colZCurrency.Caption = "Валюта";
			this.colZCurrency.FieldName = "ZCurrency";
			this.colZCurrency.Name = "colZCurrency";
			this.colZCurrency.OptionsColumn.AllowEdit = false;
			this.colZCurrency.Visible = true;
			this.colZCurrency.VisibleIndex = 4;
			// 
			// colZUnit
			// 
			this.colZUnit.Caption = "Ед. измерения";
			this.colZUnit.FieldName = "ZUnit";
			this.colZUnit.Name = "colZUnit";
			this.colZUnit.OptionsColumn.AllowEdit = false;
			this.colZUnit.Visible = true;
			this.colZUnit.VisibleIndex = 5;
			// 
			// colZVolume
			// 
			this.colZVolume.Caption = "Цех. уп.";
			this.colZVolume.FieldName = "ZVolume";
			this.colZVolume.Name = "colZVolume";
			this.colZVolume.OptionsColumn.AllowEdit = false;
			this.colZVolume.Visible = true;
			this.colZVolume.VisibleIndex = 6;
			// 
			// colZQuantity
			// 
			this.colZQuantity.Caption = "Количество";
			this.colZQuantity.FieldName = "ZQuantity";
			this.colZQuantity.Name = "colZQuantity";
			this.colZQuantity.OptionsColumn.AllowEdit = false;
			this.colZQuantity.Visible = true;
			this.colZQuantity.VisibleIndex = 7;
			// 
			// colZPeriod
			// 
			this.colZPeriod.Caption = "Срок годности";
			this.colZPeriod.FieldName = "ZPeriod";
			this.colZPeriod.Name = "colZPeriod";
			this.colZPeriod.OptionsColumn.AllowEdit = false;
			this.colZPeriod.Visible = true;
			this.colZPeriod.VisibleIndex = 8;
			// 
			// tpForb
			// 
			this.tpForb.Controls.Add(this.ForbGridControl);
			this.tpForb.Location = new System.Drawing.Point(4, 22);
			this.tpForb.Name = "tpForb";
			this.tpForb.Size = new System.Drawing.Size(712, 705);
			this.tpForb.TabIndex = 3;
			this.tpForb.Text = "Запрещенные";
			this.tpForb.Visible = false;
			// 
			// ForbGridControl
			// 
			this.ForbGridControl.DataSource = this.dtForb;
			this.ForbGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			// 
			// 
			// 
			this.ForbGridControl.EmbeddedNavigator.Name = "";
			this.ForbGridControl.Location = new System.Drawing.Point(0, 0);
			this.ForbGridControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
			this.ForbGridControl.LookAndFeel.UseDefaultLookAndFeel = false;
			this.ForbGridControl.LookAndFeel.UseWindowsXPTheme = true;
			this.ForbGridControl.MainView = this.gvForb;
			this.ForbGridControl.Name = "ForbGridControl";
			this.ForbGridControl.Size = new System.Drawing.Size(712, 705);
			this.ForbGridControl.TabIndex = 0;
			this.ForbGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvForb});
			// 
			// gvForb
			// 
			this.gvForb.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colFForb});
			this.gvForb.GridControl = this.ForbGridControl;
			this.gvForb.Name = "gvForb";
			this.gvForb.OptionsMenu.EnableColumnMenu = false;
			this.gvForb.OptionsMenu.EnableFooterMenu = false;
			this.gvForb.OptionsMenu.EnableGroupPanelMenu = false;
			this.gvForb.OptionsView.ShowGroupPanel = false;
			// 
			// colFForb
			// 
			this.colFForb.Caption = "Запрещенные";
			this.colFForb.FieldName = "FForb";
			this.colFForb.Name = "colFForb";
			this.colFForb.OptionsColumn.AllowEdit = false;
			this.colFForb.Visible = true;
			this.colFForb.VisibleIndex = 0;
			// 
			// menuItem1
			// 
			this.menuItem1.Checked = true;
			this.menuItem1.Index = -1;
			this.menuItem1.Text = "Упреждающий поиск";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = -1;
			this.menuItem2.Text = "Показывать сопоставление";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = -1;
			this.menuItem3.Text = "Отослать письмо о товаре";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = -1;
			this.menuItem4.Text = "Отослать письмо о изготовителях";
			// 
			// dataGridTextBoxColumn10
			// 
			this.dataGridTextBoxColumn10.Format = "";
			this.dataGridTextBoxColumn10.FormatInfo = null;
			this.dataGridTextBoxColumn10.MappingName = "UEColumn1";
			this.dataGridTextBoxColumn10.Width = 75;
			// 
			// dataGridTextBoxColumn11
			// 
			this.dataGridTextBoxColumn11.Format = "";
			this.dataGridTextBoxColumn11.FormatInfo = null;
			this.dataGridTextBoxColumn11.MappingName = "UEColumn2";
			this.dataGridTextBoxColumn11.Width = 75;
			// 
			// dataGridTextBoxColumn12
			// 
			this.dataGridTextBoxColumn12.Format = "";
			this.dataGridTextBoxColumn12.FormatInfo = null;
			this.dataGridTextBoxColumn12.MappingName = "UEColumn3";
			this.dataGridTextBoxColumn12.Width = 75;
			// 
			// dataGridTextBoxColumn13
			// 
			this.dataGridTextBoxColumn13.Format = "";
			this.dataGridTextBoxColumn13.FormatInfo = null;
			this.dataGridTextBoxColumn13.HeaderText = "Код";
			this.dataGridTextBoxColumn13.MappingName = "UECode";
			this.dataGridTextBoxColumn13.Width = 75;
			// 
			// dataGridTextBoxColumn14
			// 
			this.dataGridTextBoxColumn14.Format = "";
			this.dataGridTextBoxColumn14.FormatInfo = null;
			this.dataGridTextBoxColumn14.HeaderText = "Код производителя";
			this.dataGridTextBoxColumn14.MappingName = "UECodeCr";
			this.dataGridTextBoxColumn14.Width = 75;
			// 
			// dataGridTextBoxColumn15
			// 
			this.dataGridTextBoxColumn15.Format = "";
			this.dataGridTextBoxColumn15.FormatInfo = null;
			this.dataGridTextBoxColumn15.HeaderText = "Наименование";
			this.dataGridTextBoxColumn15.MappingName = "UEName1";
			this.dataGridTextBoxColumn15.Width = 75;
			// 
			// dataGridTextBoxColumn16
			// 
			this.dataGridTextBoxColumn16.Format = "";
			this.dataGridTextBoxColumn16.FormatInfo = null;
			this.dataGridTextBoxColumn16.HeaderText = "Производитель";
			this.dataGridTextBoxColumn16.MappingName = "UEFirmCr";
			this.dataGridTextBoxColumn16.Width = 75;
			// 
			// dataGridTextBoxColumn26
			// 
			this.dataGridTextBoxColumn26.Format = "";
			this.dataGridTextBoxColumn26.FormatInfo = null;
			this.dataGridTextBoxColumn26.HeaderText = "Валюта";
			this.dataGridTextBoxColumn26.MappingName = "UECurrency";
			this.dataGridTextBoxColumn26.Width = 75;
			// 
			// dataGridTextBoxColumn27
			// 
			this.dataGridTextBoxColumn27.Format = "";
			this.dataGridTextBoxColumn27.FormatInfo = null;
			this.dataGridTextBoxColumn27.HeaderText = "Базовая цена";
			this.dataGridTextBoxColumn27.MappingName = "UEBaseCost";
			this.dataGridTextBoxColumn27.Width = 75;
			// 
			// dataGridTextBoxColumn28
			// 
			this.dataGridTextBoxColumn28.Format = "";
			this.dataGridTextBoxColumn28.FormatInfo = null;
			this.dataGridTextBoxColumn28.HeaderText = "Ед. измерения";
			this.dataGridTextBoxColumn28.MappingName = "UEUnit";
			this.dataGridTextBoxColumn28.Width = 75;
			// 
			// dataGridTextBoxColumn29
			// 
			this.dataGridTextBoxColumn29.Format = "";
			this.dataGridTextBoxColumn29.FormatInfo = null;
			this.dataGridTextBoxColumn29.HeaderText = "Цех. уп.";
			this.dataGridTextBoxColumn29.MappingName = "UEVolume";
			this.dataGridTextBoxColumn29.Width = 75;
			// 
			// dataGridTextBoxColumn30
			// 
			this.dataGridTextBoxColumn30.Format = "";
			this.dataGridTextBoxColumn30.FormatInfo = null;
			this.dataGridTextBoxColumn30.HeaderText = "Количество";
			this.dataGridTextBoxColumn30.MappingName = "UEQuantity";
			this.dataGridTextBoxColumn30.Width = 75;
			// 
			// dataGridTextBoxColumn31
			// 
			this.dataGridTextBoxColumn31.Format = "";
			this.dataGridTextBoxColumn31.FormatInfo = null;
			this.dataGridTextBoxColumn31.HeaderText = "Срок годности";
			this.dataGridTextBoxColumn31.MappingName = "UEPeriod";
			this.dataGridTextBoxColumn31.Width = 75;
			// 
			// dataGridTextBoxColumn32
			// 
			this.dataGridTextBoxColumn32.Format = "";
			this.dataGridTextBoxColumn32.FormatInfo = null;
			this.dataGridTextBoxColumn32.HeaderText = "Признак";
			this.dataGridTextBoxColumn32.MappingName = "UEJunk";
			this.dataGridTextBoxColumn32.Width = 75;
			// 
			// colCColumn11
			// 
			this.colCColumn11.FieldName = "CColumn1";
			this.colCColumn11.Name = "colCColumn11";
			this.colCColumn11.Visible = true;
			this.colCColumn11.VisibleIndex = 0;
			// 
			// MainTimer
			// 
			this.MainTimer.Enabled = true;
			this.MainTimer.Interval = 15000;
			this.MainTimer.Tick += new System.EventHandler(this.MainTimer_Tick);
			// 
			// frmUEEMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(720, 753);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.statusBar1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.IsMdiContainer = true;
			this.KeyPreview = true;
			this.Name = "frmUEEMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Редактор нераспознанных выражений";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmUEEMain_Closing);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmUEEMain_KeyDown);
			this.Load += new System.EventHandler(this.frmUEEMain_Load);
			((System.ComponentModel.ISupportInitialize)(this.gvCatForm)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.CatalogGridControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dsMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtJobs)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtUnrecExp)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtZero)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtForb)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtRegions)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtCatalogFirmCr)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtSections)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtCatalogNames)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtCatalog)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtProducts)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gvCatalog)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gvProducts)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpAll)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpCurrent)).EndInit();
			this.panel3.ResumeLayout(false);
			this.tcMain.ResumeLayout(false);
			this.tpJobs.ResumeLayout(false);
			this.pnlCenter1.ResumeLayout(false);
			this.pnlTop1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.JobsGridControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gvJobs)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.pnlWithButton1.ResumeLayout(false);
			this.tpUnrecExp.ResumeLayout(false);
			this.pnlBottom2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.UnrecExpGridControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gvUnrecExp)).EndInit();
			this.pnlCenter2.ResumeLayout(false);
			this.pnlLeft2.ResumeLayout(false);
			this.grpBoxCatalog2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcFirmCr)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gvFirmCr)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
			this.pnlTop2.ResumeLayout(false);
			this.tpZero.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ZeroGridControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gvZero)).EndInit();
			this.tpForb.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ForbGridControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gvForb)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private DevExpress.XtraGrid.GridControl gcFirmCr;
		private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
		private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
		private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
		private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
		private DevExpress.XtraGrid.Views.Grid.GridView gvFirmCr;
		private DevExpress.XtraGrid.Columns.GridColumn colFirmCrName;
		private System.Data.DataColumn colCatalogProductsCount;
		private DevExpress.XtraGrid.Columns.GridColumn colFProductsCount;
	}
}
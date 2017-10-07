namespace Microsoft.Dynamics.Samples.AddIns.DeveloperToolsAddin1
{
    using System;
    using System.Linq;
    using System.ComponentModel.Composition;
    using Microsoft.Dynamics.AX.Metadata.Core;
    using Microsoft.Dynamics.Framework.Tools.Extensibility;
    using Microsoft.Dynamics.Framework.Tools.MetaModel.Automation;
    using Microsoft.Dynamics.Framework.Tools.MetaModel.Automation.Forms;
    using Microsoft.Dynamics.Framework.Tools.MetaModel.Automation.Tables;
    using Microsoft.Dynamics.Framework.Tools.MetaModel.Core;

    using AX = Microsoft.Dynamics.AX;

    // Convenience prefixes
    using TablesAutomation = Microsoft.Dynamics.Framework.Tools.MetaModel.Automation.Tables;
    using ViewsAutomation = Microsoft.Dynamics.Framework.Tools.MetaModel.Automation.Views;
    using Metadata = Microsoft.Dynamics.AX.Metadata;
    using AX.Metadata.MetaModel;
    using global::DeveloperToolsAddin1;
    using System.Text;

    /// <summary>
    /// TODO: Say a few words about what your AddIn is going to do
    /// </summary>
    [Export(typeof(IDesignerMenu))]
    // TODO: This addin will show when user right clicks on a form root node or table root node. 
    // If you need to specify any other element, change this AutomationNodeType value.
    // You can specify multiple DesignerMenuExportMetadata attributes to meet your needs
    [DesignerMenuExportMetadata(AutomationNodeType = typeof(IForm))]
    [DesignerMenuExportMetadata(AutomationNodeType = typeof(ITable))]
    public class DesignerContextMenuAddIn : DesignerMenuBase
    {
        #region Member variables
        private const string addinName = "Table Fields";
        #endregion

        #region Properties
        /// <summary>
        /// Caption for the menu item. This is what users would see in the menu.
        /// </summary>
        public override string Caption
        {
            get
            {
                return AddinResources.DesignerAddinCaption;
            }
        }
        private Metadata.Providers.IMetadataProvider metadataProvider = null;

        public Metadata.Providers.IMetadataProvider MetadataProvider
        {
            get
            {
                if (this.metadataProvider == null)
                {
                    this.metadataProvider = DesignMetaModelService.Instance.CurrentMetadataProvider;
                }
                return this.metadataProvider;
            }
        }


        /// <summary>
        /// Unique name of the add-in
        /// </summary>
        public override string Name
        {
            get
            {
                return DesignerContextMenuAddIn.addinName;
            }
        }
        #endregion


        // Create a new method with empty body
        private AxMethod BuildMethod(string name)
        {
            AxMethod axMethod = new AxMethod()
            {
                Name = name,
                ReturnType = new AxMethodReturnType(),
                
            };

            return axMethod;
        }
        private StringBuilder GenerateFromTable(TablesAutomation.ITable selectedTable)
        {
            AxTable table = this.MetadataProvider.Tables.Read(selectedTable.Name);
            var result = new StringBuilder();
            foreach (AxTableField field in table.Fields)
            {
                result.AppendLine(field.Name);
            }

            // It is indeed a table. Look at the properties

            return result;
        }
        string src()
        {
            return @"    boolean find(XcfDataTableActionType _actionType)
    {
        boolean ret = false;
        ;
    
        switch (_actionType)
        {
            case XcfDataTableActionType::Generate :
                ret = true;
                break;
    
            case XcfDataTableActionType::Export :
                ret = this.generateLogRecId() != 0;
                break;
    
            case XcfDataTableActionType::Cleanup :
                ret = this.generateLogRecId() != 0;
                break;
    
            default :
                throw error(XcfStatic::missingKey(_actionType));
        }
    
        return ret;
    }
";
        }
        void createFind(TablesAutomation.ITable selectedTable)
        {

            AxTable table = this.MetadataProvider.Tables.Read(selectedTable.Name);
            AxMethod method;

            method = table.Methods.FirstOrDefault(x => x.Name == "find");

            if (method == null)
            {
                AxMethod m = this.BuildMethod("find");
                m.ReturnType.TypeName = "boolean";
                m.Source = src();
                table.AddMethod(m);
                System.Windows.Forms.MessageBox.Show("dodano find");
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("nie dodano find");
                return;
            }

            var metaModelProviders = ServiceLocator.GetService(typeof(IMetaModelProviders)) as IMetaModelProviders;
            if (metaModelProviders == null)
                throw new Exception("A");

            var metaModelService = metaModelProviders.CurrentMetaModelService;
            if (metaModelService == null)
                throw new Exception("B");
            // Getting the model will likely have to be more sophisticated, such as getting the model of the project and checking
            // if the object has the same model.
            // But this should do for demonstration.

            ModelInfo model = DesignMetaModelService.Instance.CurrentMetadataProvider.Tables.GetModelInfo(table.Name).FirstOrDefault<ModelInfo>();
            if (model == null)
                throw new Exception("C");

            // Update the file
            metaModelService.UpdateTable(table, new ModelSaveInfo(model));
        }

        #region Callbacks
        /// <summary>
        /// Called when user clicks on the add-in menu
        /// </summary>
        /// <param name="e">The context of the VS tools and metadata</param>
        public override void OnClick(AddinDesignerEventArgs e)
        {
            try
            {
                StringBuilder result = null;
                TablesAutomation.ITable selectedTable;

                if ((selectedTable = e.SelectedElement as TablesAutomation.ITable) != null)
                {
                    createFind(selectedTable);
                    //result = GenerateFromTable(selectedTable);
                    //System.Windows.Forms.MessageBox.Show(result.ToString());
                }
            }
            catch (Exception ex)
            {
                CoreUtility.HandleExceptionWithErrorMessage(ex);
            }
        }
        #endregion
    }
}

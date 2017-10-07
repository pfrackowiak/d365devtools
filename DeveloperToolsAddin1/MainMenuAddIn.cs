namespace Microsoft.Dynamics.Samples.AddIns.DeveloperToolsAddin1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using Microsoft.Dynamics.Framework.Tools.Extensibility;
    using Microsoft.Dynamics.Framework.Tools.MetaModel.Core;

    using AX = Microsoft.Dynamics.AX;

    // Convenience prefixes
    using TablesAutomation = Microsoft.Dynamics.Framework.Tools.MetaModel.Automation.Tables;
    using ViewsAutomation = Microsoft.Dynamics.Framework.Tools.MetaModel.Automation.Views;
    using Metadata = Microsoft.Dynamics.AX.Metadata;
    using AX.Metadata.MetaModel;
    using global::DeveloperToolsAddin1;

    /// <summary>
    /// TODO: Say a few words about what your AddIn is going to do
    /// </summary>
    [Export(typeof(IMainMenu))]
    public class MainMenuAddIn : DesignerMenuBase
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
                return AddinResources.MainMenuAddInCaption;
            }
        }

        /// <summary>
        /// Unique name of the add-in
        /// </summary>
        public override string Name
        {
            get
            {
                return MainMenuAddIn.addinName;
            }
        }

        #endregion

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


        #region Callbacks
        /// <summary>
        /// Called when user clicks on the add-in menu
        /// </summary>
        /// <param name="e">The context of the VS tools and metadata</param>
        private StringBuilder GenerateFromTable(TablesAutomation.ITable selectedTable)
        {
            var result = new StringBuilder();
            AxTable table = this.MetadataProvider.Tables.Read(selectedTable.Name);
            foreach (AxTableField field in table.Fields)
            {
                result.AppendLine(field.Name);
            }

            // It is indeed a table. Look at the properties

            return result;
        }

        public override void OnClick(AddinDesignerEventArgs e)
        {
            try
            {
                StringBuilder result = null;
                TablesAutomation.ITable selectedTable;

                if ((selectedTable = e.SelectedElement as TablesAutomation.ITable) != null)
                {
                    result = this.GenerateFromTable(selectedTable);
                    System.Windows.Forms.MessageBox.Show(result.ToString());
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

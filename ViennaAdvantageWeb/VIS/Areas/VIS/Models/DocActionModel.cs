﻿/********************************************************
 * Project Name   : VIS
 * Class Name     : DocActionModel
 * Purpose        : Return Doc Actions , depends upon current doc status and tableName.
 * Chronological    Development
 * Karan            
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.Utility;
using VAdvantage.WF;

namespace VIS.Models
{
    public class DocActionModel
    {
        Ctx ctx = null;
        public DocActionModel(Ctx ctx)
        {
            this.ctx = ctx;
        }

        private static String[] _moduleClasses = new String[]{
            ".Common.",
            ".Model.M",
            ".Process.M",
            ".WF.M",
            ".Report.M",
            ".Print.M",
            ".CMFG.Model.M",
            ".CMRP.Model.M",
            ".CWMS.Model.M",
            ".Model.X_",};



        public DocAtions GetActions(int AD_Table_ID, int Record_ID, string docStatus, bool processing, string orderType, bool isSOTrx, string docAction, string tableName, List<string> _values, List<string> _names)
        {
            DocAtions action = new DocAtions();
            string[] options = null;
            int index = 0;
            string defaultV = "";
            action.DocStatus = docStatus;

            VLogger.Get().Fine("DocStatus=" + docStatus
               + ", DocAction=" + docAction + ", OrderType=" + orderType
               + ", IsSOTrx=" + isSOTrx + ", Processing=" + processing
               + ", AD_Table_ID=" + AD_Table_ID + ", Record_ID=" + Record_ID);
            options = new String[_values.Count()];
            String wfStatus = MWFActivity.GetActiveInfo(ctx, AD_Table_ID, Record_ID);
            if (wfStatus != null)
            {
                VLogger.Get().SaveError("WFActiveForRecord", wfStatus);
                action.Error = "WFActiveForRecord";
                return action;
            }

            //	Status Change
            if (!CheckStatus(tableName, Record_ID, docStatus))
            {
                VLogger.Get().SaveError("DocumentStatusChanged", "");
                action.Error = "DocumentStatusChanged";
                return action;
            }
           // if (processing != null)
            {
                bool locked = "Y".Equals(processing);
                if (!locked && processing.GetType() == typeof(Boolean))
                    locked = ((Boolean)processing);
                if (locked)
                    options[index++] = DocumentEngine.ACTION_UNLOCK;
            }
           
            //	Approval required           ..  NA
            if (docStatus.Equals(DocumentEngine.STATUS_NOTAPPROVED))
            {
                options[index++] = DocumentEngine.ACTION_PREPARE;
                options[index++] = DocumentEngine.ACTION_VOID;
            }
            //	Draft/Invalid				..  DR/IN
            else if (docStatus.Equals(DocumentEngine.STATUS_DRAFTED)
                || docStatus.Equals(DocumentEngine.STATUS_INVALID))
            {
                options[index++] = DocumentEngine.ACTION_COMPLETE;
                //	options[index++] = DocumentEngine.ACTION_Prepare;
                options[index++] = DocumentEngine.ACTION_VOID;
            }
            //	In Process                  ..  IP
            else if (docStatus.Equals(DocumentEngine.STATUS_INPROGRESS)
                || docStatus.Equals(DocumentEngine.STATUS_APPROVED))
            {
                options[index++] = DocumentEngine.ACTION_COMPLETE;
                options[index++] = DocumentEngine.ACTION_VOID;
            }
            //	Complete                    ..  CO
            else if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
            {
                options[index++] = DocumentEngine.ACTION_CLOSE;
            }
            //	Waiting Payment
            else if (docStatus.Equals(DocumentEngine.STATUS_WAITINGPAYMENT)
                || docStatus.Equals(DocumentEngine.STATUS_WAITINGCONFIRMATION))
            {
                options[index++] = DocumentEngine.ACTION_VOID;
                options[index++] = DocumentEngine.ACTION_PREPARE;
            }
            //	Closed, Voided, REversed    ..  CL/VO/RE
            else if (docStatus.Equals(DocumentEngine.STATUS_CLOSED)
                || docStatus.Equals(DocumentEngine.STATUS_VOIDED)
                || docStatus.Equals(DocumentEngine.STATUS_REVERSED))

                return action;

            GetActionFromModuleClass(AD_Table_ID, docStatus, index, options);

            /********************
             *  Order
             */
            if (AD_Table_ID == MOrder.Table_ID)
            {
                //	Draft                       ..  DR/IP/IN
                if (docStatus.Equals(DocumentEngine.STATUS_DRAFTED)
                    || docStatus.Equals(DocumentEngine.STATUS_INPROGRESS)
                    || docStatus.Equals(DocumentEngine.STATUS_INVALID))
                {
                    options[index++] = DocumentEngine.ACTION_PREPARE;
                    options[index++] = DocumentEngine.ACTION_CLOSE;
                    //	Draft Sales Order Quote/Proposal - Process
                    if (isSOTrx
                        && ("OB".Equals(orderType) || "ON".Equals(orderType)))
                        docAction = DocumentEngine.ACTION_PREPARE;
                }
                //	Complete                    ..  CO
                else if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    options[index++] = DocumentEngine.ACTION_VOID;
                    options[index++] = DocumentEngine.ACTION_REACTIVATE;
                }
                else if (docStatus.Equals(DocumentEngine.STATUS_WAITINGPAYMENT))
                {
                    options[index++] = DocumentEngine.ACTION_REACTIVATE;
                    options[index++] = DocumentEngine.ACTION_CLOSE;
                }
            }
            /********************
             *  Shipment
             */
            else if (AD_Table_ID == MInOut.Table_ID)
            {
                //	Complete                    ..  CO
                if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    options[index++] = DocumentEngine.ACTION_VOID;
                    options[index++] = DocumentEngine.ACTION_REVERSE_CORRECT;
                }
            }
            /********************
             *  Invoice
             */
            else if (AD_Table_ID == MInvoice.Table_ID)
            {
                //	Complete                    ..  CO
                if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    options[index++] = DocumentEngine.ACTION_VOID;
                    options[index++] = DocumentEngine.ACTION_REVERSE_CORRECT;
                }
            }
            /********************
             *  Payment
             */
            else if (AD_Table_ID == MPayment.Table_ID)
            {
                //	Complete                    ..  CO
                if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    options[index++] = DocumentEngine.ACTION_VOID;
                    options[index++] = DocumentEngine.ACTION_REVERSE_CORRECT;
                }
            }
            /********************
             *  GL Journal
             */
            //else if (AD_Table_ID == MJournal.Table_ID || AD_Table_ID == MJournalBatch.Table_ID)
            //{
            //    //	Complete                    ..  CO
            //    if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
            //    {
            //        options[index++] = DocumentEngine.ACTION_REVERSE_CORRECT;
            //        options[index++] = DocumentEngine.ACTION_REVERSE_ACCRUAL;
            //    }
            //}
            /********************
             *  Allocation
             */
            else if (AD_Table_ID == MAllocationHdr.Table_ID)
            {
                //	Complete                    ..  CO
                if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    options[index++] = DocumentEngine.ACTION_VOID;
                    options[index++] = DocumentEngine.ACTION_REVERSE_CORRECT;
                }
            }
            /********************
             *  Bank Statement
             */
            else if (AD_Table_ID == MBankStatement.Table_ID)
            {
                //	Complete                    ..  CO
                if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    options[index++] = DocumentEngine.ACTION_VOID;
                }
            }
            /********************
             *  Inventory Movement, Physical Inventory
             */
            else if (AD_Table_ID == MMovement.Table_ID
                || AD_Table_ID == MInventory.Table_ID)
            {
                //	Complete                    ..  CO
                if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    options[index++] = DocumentEngine.ACTION_VOID;
                    options[index++] = DocumentEngine.ACTION_REVERSE_CORRECT;
                }
            }

                //    /********************
            //*  Warehouse Task  New Add by raghu 11 april,2011
            //*/
            //    else if (AD_Table_ID == X_M_WarehouseTask.Table_ID
            //        || AD_Table_ID == X_M_TaskList.Table_ID)
            //    {
            //        //	Draft                       ..  DR/IP/IN
            //        if (docStatus.Equals(DocActionVariables.STATUS_DRAFTED)
            //            || docStatus.Equals(DocActionVariables.STATUS_INPROGRESS)
            //            || docStatus.Equals(DocActionVariables.STATUS_INVALID))
            //        {
            //            options[index++] = DocActionVariables.ACTION_PREPARE;
            //        }
            //        //	Complete                    ..  CO
            //        else if (docStatus.Equals(DocActionVariables.STATUS_COMPLETED))
            //        {
            //            options[index++] = DocActionVariables.ACTION_VOID;
            //            options[index++] = DocActionVariables.ACTION_REVERSE_CORRECT;
            //        }
            //    }
            /********************
         *  Work Order New Add by raghu 11 april,2011
         */
            else if (AD_Table_ID == ViennaAdvantage.Model.X_VAMFG_M_WorkOrder.Table_ID)
            {
                //	Draft                       ..  DR/IP/IN
                if (docStatus.Equals(DocActionVariables.STATUS_DRAFTED)
                    || docStatus.Equals(DocActionVariables.STATUS_INPROGRESS)
                    || docStatus.Equals(DocActionVariables.STATUS_INVALID))
                {
                    options[index++] = DocActionVariables.ACTION_PREPARE;
                }
                //	Complete                    ..  CO
                else if (docStatus.Equals(DocActionVariables.STATUS_COMPLETED))
                {
                    options[index++] = DocActionVariables.ACTION_VOID;
                    options[index++] = DocActionVariables.ACTION_REACTIVATE;
                }
            }
            /********************
             *  Work Order Transaction New Add by raghu 11 april,2011
             */
            else if (AD_Table_ID == ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.Table_ID)
            {
                //	Draft                       ..  DR/IP/IN
                if (docStatus.Equals(DocActionVariables.STATUS_DRAFTED)
                    || docStatus.Equals(DocActionVariables.STATUS_INPROGRESS)
                    || docStatus.Equals(DocActionVariables.STATUS_INVALID))
                {
                    options[index++] = DocActionVariables.ACTION_PREPARE;
                }
                //	Complete                    ..  CO
                else if (docStatus.Equals(DocActionVariables.STATUS_COMPLETED))
                {
                    options[index++] = DocActionVariables.ACTION_VOID;
                    options[index++] = DocActionVariables.ACTION_REVERSE_CORRECT;
                }
            }

            /***For Primary thread***/
            ///**
            // *	Fill actionCombo
            // */
            //for (int i = 0; i < index; i++)
            //{
            //    //	Serach for option and add it
            //    bool added = false;
            //    for (int j = 0; j < _values.Length && !added; j++)
            //        if (options[i].Equals(_values[j]))
            //        {
            //            //actionCombo.addItem(_names[j]);
            //            vcmbAction.Items.Add(_names[j]);
            //            added = true;
            //        }
            //}

            //	setDefault
            if (docAction.Equals("--"))		//	If None, suggest closing
                docAction = DocumentEngine.ACTION_CLOSE;

            for (int i = 0; i < _values.Count() && defaultV.Equals(""); i++)
                if (docAction.Equals(_values[i]))
                    defaultV = _names[i];


            action.Options = options.ToList();
            action.Index = index;
            action.DefaultV = defaultV;
            

            return action;

            /***For Primary thread***/
            //if (!defaultV.Equals(""))
            //{
            //    //vcmbAction.SelectedValue = defaultV;
            //    vcmbAction.SelectedItem = defaultV;
            //}


        }


        /// <summary>
        /// Check Status Change
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="Record_ID">record id</param>
        /// <param name="docStatus">current doc status</param>
        /// <returns>true if status not changed</returns>
        private bool CheckStatus(String tableName, int Record_ID, String docStatus)
        {
            String sql = "SELECT COUNT(*) FROM " + tableName
                + " WHERE " + tableName + "_ID=" + Record_ID
                + " AND DocStatus='" + docStatus + "'";
            int result = Util.GetValueOfInt(DB.ExecuteScalar(sql));
            return result > 0;
        }


        public void GetActionFromModuleClass(int AD_Table_ID, string docStatus, int index, string[] options)
        {
            /*********** Module Section  **************/


            #region GetActionFromModuleClass


            MTable mTable = new MTable(ctx, AD_Table_ID, null);
            //	Strip table name prefix (e.g. AD_) Customizations are 3/4
            String classNm = "DocActionSpecification";


            Tuple<String, String,String> moduleInfo;
            Assembly asm = null;
            string namspace = "";
            if (Env.HasModulePrefix(mTable.GetTableName(), out  moduleInfo))
            {
                asm = null;
                try
                {
                    asm = Assembly.Load(moduleInfo.Item1);
                }
                catch (Exception e)
                {
                    VLogger.Get().Info(e.Message);
                    asm = null;
                }

                if (asm != null)
                {
                    for (int i = 0; i < _moduleClasses.Length; i++)
                    {
                        namspace = moduleInfo.Item2 + _moduleClasses[i] + classNm;
                        if (_moduleClasses.Contains("X_"))
                        {
                            namspace = moduleInfo.Item2 + _moduleClasses[i] + mTable.GetTableName();
                        }

                        Type clazzsn = GetClassFromAsembly(asm, namspace);
                        if (clazzsn != null)
                        {
                            ConstructorInfo constructor = null;
                            try
                            {
                                constructor = clazzsn.GetConstructor(new Type[] { });
                            }
                            catch (Exception e)
                            {
                                VLogger.Get().Warning("No transaction Constructor for " + clazzsn.FullName + " (" + e.ToString() + ")");
                            }
                            if (constructor != null)
                            {
                                object o = constructor.Invoke(null);
                                if (o is ModuleDocAction)
                                {
                                    string[] opt = ((ModuleDocAction)o).GetDocAtion(AD_Table_ID, docStatus);// .Invoke(new object[] { AD_Table_ID, docStatus });
                                    if (opt.Length > 0)
                                    {
                                        index = 0;
                                    }
                                    for (int j = 0; j < opt.Length; j++)
                                    {
                                        options[index++] = opt[j];

                                    }

                                    break;
                                }
                            }
                            //return clazzsn;
                        }

                    }
                }
            }
            #endregion
            /*********** END  **************/
        }


        /// <summary>
        /// Get Class From Assembly
        /// </summary>
        /// <param name="asm">Assembly</param>
        /// <param name="className">Fully Qulified Class Name</param>
        /// <returns>Class Object</returns>
        private Type GetClassFromAsembly(Assembly asm, string className)
        {
            Type type = null;
            try
            {
                type = asm.GetType(className);
            }
            catch (Exception e)
            {
                VLogger.Get().Log(Level.SEVERE, e.Message);
            }
            //return type;
            if (type == null)
            {
                return null;
            }

            Type baseClass = type.BaseType;

            while (baseClass != null)
            {
                if (baseClass == typeof(ModuleDocAction) || baseClass == typeof(object))
                {
                    return type;
                }
                baseClass = baseClass.BaseType;
            }
            return null;
        }

    }

    public interface ModuleDocAction
    {
        String[] GetDocAtion(int AD_Table_ID, String docStatus);

    }

    public class DocAtions
    {
        public List<string> Options { get; set; }
        public int Index { get; set; }
        public string DefaultV { get; set; }
        public string DocStatus { get; set; }
        public string Error { get; set; }
    }

}
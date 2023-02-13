

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using GenericRecipe.Algorithm;
using System.Runtime.Serialization;
using GenericRecipe.GenericType;
using static GenericRecipe.GenericType.GenericTuple;
using System.Windows.Forms;
using static GenericRecipe.RecipeClass;
using System.Reflection;

namespace GenericRecipe
{
    
    internal static class TreeNodesExtensions
    {
        internal static void m_MappingTreeNodes(this TreeNode distNode, TreeNode srcnode)
        {
            foreach (TreeNode node in srcnode.Nodes)
            {
                if (!distNode.Nodes.ContainsKey(node.Name))
                {
                    distNode.Nodes.Add(node);
                }
                else
                    if (node.Nodes.Count > 0)
                    distNode.Nodes[node.Name].m_MappingTreeNodes(node);
            }


        }


        internal static void MoveUp(this TreeNode node)
        {
            TreeNode parent = node.Parent;
            TreeView view = node.TreeView;
            if (parent != null)
            {
                int index = parent.Nodes.IndexOf(node);
                if (index > 0)
                {
                    parent.Nodes.RemoveAt(index);
                    parent.Nodes.Insert(index - 1, node);
                }
            }
            else if (node.TreeView.Nodes.Contains(node)) //root node
            {
                int index = view.Nodes.IndexOf(node);
                if (index > 0)
                {
                    view.Nodes.RemoveAt(index);
                    view.Nodes.Insert(index - 1, node);
                }
            }
        }

        internal static void MoveDown(this TreeNode node)
        {
            TreeNode parent = node.Parent;
            TreeView view = node.TreeView;
            if (parent != null)
            {
                int index = parent.Nodes.IndexOf(node);
                if (index < parent.Nodes.Count - 1)
                {
                    parent.Nodes.RemoveAt(index);
                    parent.Nodes.Insert(index + 1, node);
                }
            }
            else if (view != null && view.Nodes.Contains(node)) //root node
            {
                int index = view.Nodes.IndexOf(node);
                if (index < view.Nodes.Count - 1)
                {
                    view.Nodes.RemoveAt(index);
                    view.Nodes.Insert(index + 1, node);
                }
            }
        }
        //struct TreeNodeExpandStatus
        //{
        //    public bool IsExpanded;
        //    public string Name;
        //    public TreeNodeExpandStatus[] Nodes;
        //}
    }
    internal static class RandomStringExtensions
    {
        private static Random random = new Random();
        
        public static string RandomString(this string str, int length)
        {
            if (str.Length == 0)
                return "";

            string chars = str;
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
    //using GenericDictionary = Dictionary<string, GenericTuple>;

    public static class GenericRecipeEditControlExtensions
    {
        private static string[] CannotEditNodes = new string[] { "InspParams","RecipeParams","StorableParams", "UnstorableParams" , "Algorithm" };
        private static string[] CannottEditableWhenParentWasThoseNodes = new string[] { "Algorithm" };
        private static string[] CanEditableNodesUnderThoseNodes = new string[] { "StorableParams", "UnstorableParams" };
        private static string[] CanSortableNodesWhenParentWasThoseNodes = new string[] { "Algorithm" };

        //private static Type[] RemovableTypeNodees = new Type[] { typeof(GenericDictionary),typeof(GenericTuple),typeof(IAlgorithmClass) };

        private static Color Color_EditError = Color.HotPink;
        private static Color Color_EditTypeError = Color.Orange;
        private static Color Color_Changed = Color.LightGreen;
        private static Color Color_Normal = Color.White;

        private const string Btn_ADD = "btn_Add";
        private const string Btn_REMOVE = "btn_Remove";
        private const string Cmb_ADD = "cmb_Add";
        private const string Btn_SAVE = "btn_Save";
        private const string Btn_SORTUP = "btn_SortUp";
        private const string Btn_SORTDOWN = "btn_SortDown";

        private const string Trv_RecipeTreeview = "trv_Rcp";


    

        #region "For edit form"
        public static TreeNode GetRecipeTreeNode(this RecipeClass recipe, Enum_TreeNodeParamsType treenodeType)
        {
            var node = new TreeNode();
            node.Text = "GenericRecipe";
            node.Name = "GenericRecipe";
            node.Tag = recipe;
            node.Nodes.Add(recipe.RecipeParams.m_GetTreeNode("RecipeParams", treenodeType));
            node.Nodes["RecipeParams"].Tag = recipe.RecipeParams;
            var inspParamsnode = new TreeNode();
            inspParamsnode.Tag = recipe.InspectionParams;
            inspParamsnode.Name = "InspectionParams";
            inspParamsnode.Text = "InspectionParams";

            for (int i = 0; i < recipe.InspectionParams.Count; i++)
                inspParamsnode.Nodes.Add(recipe.InspectionParams[i].m_GetTreeNode(recipe.InspectionParams[i] .GetType().Name+ " #" + i.ToString(), treenodeType));

            node.Nodes.Add(inspParamsnode);
            node.Expand();
            inspParamsnode.Expand();
            return node;
        }

        private static bool m_GenericTupleSetValueByCastFromString(GenericDictionary dict, string key, Enum_InspTupleType? type, string value)
        {
            var ret = false;

            if (type == null)
                return false;
            switch (type)
            {
                case Enum_InspTupleType.Int16:
                    ret = Int16.TryParse(value, out var vali16);
                    if (ret)
                        dict[key] = new GenericTuple(vali16);
                    break;
                case Enum_InspTupleType.Int32:
                    ret = Int32.TryParse(value, out var vali32);
                    if (ret)
                        dict[key] = new GenericTuple(vali32);
                    break;
                case Enum_InspTupleType.Int64:
                    ret = Int64.TryParse(value, out var vali64);
                    if (ret)
                        dict[key] = new GenericTuple(vali64);
                    break;
                case Enum_InspTupleType.Double:
                    ret = Double.TryParse(value, out var valdbl);
                    if (ret)
                        dict[key] = new GenericTuple(valdbl);
                    break;

                case Enum_InspTupleType.Single:
                    ret = Single.TryParse(value, out var valsng);
                    dict[key] = new GenericTuple(valsng);
                    break;
                case Enum_InspTupleType.String:
                    ret = true;
                    dict[key] = new GenericTuple(value);
                    break;
                default:
                    return false;

            }

            return ret;
        }
        private static bool m_GenericTupleStringValueTypeCheck(Enum_InspTupleType? type, string value)
        {
            if (type == null)
                return false;
            switch (type)
            {
                case Enum_InspTupleType.Int16:
                    return Int16.TryParse(value, out var vali16);
                case Enum_InspTupleType.Int32:
                    return Int32.TryParse(value, out var vali32);
                case Enum_InspTupleType.Int64:
                    return Int64.TryParse(value, out var vali64);
                case Enum_InspTupleType.Double:
                    return Double.TryParse(value, out var valdbl);
                case Enum_InspTupleType.Single:
                    return Single.TryParse(value, out var valsng);
                case Enum_InspTupleType.String:
                    return true;
                default:
                    return false;

            }
        }
        private static GenericDictionary m_Nodes_GetLastGenericDictionaryParent(TreeNode node)
        {
            if (node == null || node.Parent == null)
                return null;


            if (node.Parent.Tag.GetType() == typeof(GenericDictionary))
                return node.Parent.Tag as GenericDictionary;
            else
                return m_Nodes_GetLastGenericDictionaryParent(node.Parent);

        }




        /// <summary>
        /// This function return a EditForm (type of Form),show the form using EditForm.ShowDialog() or EditForm.Show()
        /// 
        /// How to get the modified-recipe?
        /// 1. Check the EidtForm.DialogResult = DialogResult.Ok;
        /// 2. Get the recipe at EditForm.Tag (type of RecipeClass);
        /// </summary>
        /// <param name="srcRecipe"></param>
        /// <param name="treenodeType"></param>
        /// <returns></returns>
        public static Form GetEditForm(this RecipeClass srcRecipe, Enum_TreeNodeParamsType treenodeType , bool editable = true)
        {
            var recipe = srcRecipe.Clone() as RecipeClass;
            
            Form form = new Form();
            form.Tag = recipe;
            form.SizeChanged += formSizeChange;
            
            #region "Treeview"
            TreeView treeView = new TreeView();
           
            treeView.BeforeLabelEdit += TreeView_BeforeLabelEdit;
            treeView.AfterLabelEdit += TreeView_AfterLabelEdit;
            treeView.AfterSelect += TreeView_AfterSelect; 
            //treeView.BeforeSelect
            treeView.Nodes.Add(recipe.GetRecipeTreeNode(treenodeType));

            treeView.Name = Trv_RecipeTreeview;
            form.Controls.Add(treeView);
         
            treeView.Location = new Point(5, 5);
            
            treeView.LabelEdit = true;

            #endregion


            if (editable)
                m_InitializeGenericRecipeEditControls(form);

            form.Font = new Font("Consolas", 14);
            
            form.Location = new Point(0, 0);
            form.Size = new Size((int)(Screen.PrimaryScreen.Bounds.Width * 0.8), (int)(Screen.PrimaryScreen.Bounds.Height * 0.9));
            form.DialogResult = DialogResult.None;

            
            return form;
        }

  
        public static void m_InitializeGenericRecipeEditControls(Form eidtForm)
        {
            #region "SaveButton"
            var btn_save = new Button();

            btn_save.Name = Btn_SAVE;
            btn_save.Click += Btn_save_Click;
            btn_save.Text = "Save";
            btn_save.BackColor = Color.LightBlue;



            eidtForm.Controls.Add(btn_save);

            #endregion
            #region "AddButton"
            var btn_Add = new Button();

            btn_Add.Name = Btn_ADD;
            btn_Add.Click += Btn_Add_Click;
            btn_Add.Text = "Add";
            eidtForm.Controls.Add(btn_Add);
            #endregion
            #region "Remove button"
            var btn_Remove = new Button();

            btn_Remove.Name = Btn_REMOVE;
            btn_Remove.Click += Btn_Remove_Click;
            btn_Remove.Text = "Remove";
            eidtForm.Controls.Add(btn_Remove);
            #endregion

            #region "Append combobox"
            var cmb_Add = new ComboBox();
            cmb_Add.Name = Cmb_ADD;
            eidtForm.Controls.Add(cmb_Add);
            #endregion

            #region "sort controls"
            var btn_sortUp = new Button();
            var btn_sortDown = new Button();
            btn_sortUp.Name = Btn_SORTUP;
            btn_sortDown.Name = Btn_SORTDOWN;
            btn_sortUp.Text = "︽";
            btn_sortDown.Text = "︾";
            btn_sortUp.Click += Btn_sortUp_Click;
            btn_sortDown.Click += Btn_sortDown_Click;
            eidtForm.Controls.Add(btn_sortUp);
            eidtForm.Controls.Add(btn_sortDown);
            #endregion
        }



        private static string[] m_getNodeRoadmap(TreeNode node)
        {
            List<string> roadmap = new List<string>();

            roadmap.Add(node.Name);
            if (node.Parent != null)
            {
                var ret = m_getNodeRoadmap(node.Parent);
                foreach (var name in ret)
                    roadmap.Add(name);
            }

            return roadmap.ToArray();

        }

        private static bool m_checkNodeEditable(string[] roadmap)
        {

            if (roadmap == null || roadmap.Length == 0)
                return false;

            if (CannotEditNodes.Contains(roadmap[0]))
                return false;

            if(roadmap.Length > 1)
                if (CannottEditableWhenParentWasThoseNodes.Contains(roadmap[1]) )
                    return false;




            for (int i = 1; i < roadmap.Length; i++)
                if (CanEditableNodesUnderThoseNodes.Contains(roadmap[i]))
                    return true;

            return false;
        }


        private static Type[] m_GetAllChildClasses(Type type)
        {
            Type[] ret;

            ret = Assembly.GetAssembly(type).GetTypes().Where(t =>
            {
                return t.IsSubclassOf(type);

            }

            ).ToArray();

            return ret;

        }

        private static Type[] m_GetAllAssignableClasses(Type type)
        {
            Type[] ret;

            ret = Assembly.GetAssembly(type).GetTypes().Where(t =>
            {
                return type.IsAssignableFrom(t);

            }

            ).ToArray();

            return ret;

        }
        #endregion

        #region "Control Events"

        private static void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            m_setGenericRecipeControl(e.Node.TreeView.Parent as Form, e.Node);
        }

        private static void Btn_Remove_Click(object sender, EventArgs e)
        {
           var btn  =sender as Button;
            var node = btn.Tag as TreeNode;
            if(typeof(List<IAlgorithmClass>).IsAssignableFrom(node.Parent.Tag.GetType()))
            {
               ( node.Parent.Tag as List<IAlgorithmClass>).Remove(node.Tag as IAlgorithmClass);
                var algoName = (node.Tag as IAlgorithmClass).AlgorithmName;
                var algoKey = (node.Tag as IAlgorithmClass).AlgoKeyString;
                var storableSubAlgoParamsNode = node.Parent.Parent.Nodes["StorableParams"]?.Nodes["Algorithm"]?.Nodes[algoName]?.Nodes[algoKey];
                var unstorableSubAlgoParamsNode = node.Parent.Parent.Nodes["UnstorableParams"]?.Nodes["Algorithm"]?.Nodes[algoName]?.Nodes[algoKey];
                if (storableSubAlgoParamsNode != null)
                {
                    //(storableSubAlgoParamsNode.Parent.Tag as GenericDictionary).Remove(algoKey);
                    storableSubAlgoParamsNode.Remove();

                }
                  
                if(unstorableSubAlgoParamsNode != null)
                {
                    //(unstorableSubAlgoParamsNode.Parent.Tag as GenericDictionary).Remove(algoKey);
                    unstorableSubAlgoParamsNode.Remove();
                }
                (node.Parent.Parent.Tag as GenericInspectoinPointParams).RemoveAlgorithm(node.Tag as IAlgorithmClass);
                node.Remove();
            }
            else if(typeof(List<GenericInspectionParam>).IsAssignableFrom(node.Parent.Tag.GetType()))
            {
                (node.Parent.Tag as List<GenericInspectionParam>).Remove(node.Tag as GenericInspectionParam);
                
                var paranetNode = node.Parent as TreeNode;
                node.Remove();
                for (int i = 0; i < paranetNode.Nodes.Count; i++)
                {
                    paranetNode.Nodes[i].Name = paranetNode.Nodes[i].Tag.GetType().Name + " #" + i.ToString();
                    paranetNode.Nodes[i].Text = paranetNode.Nodes[i].Tag.GetType().Name + " #" + i.ToString();
                }
            }
            else if(typeof(GenericDictionary).IsAssignableFrom(node.Parent.Tag.GetType()))
            {
                (node.Parent.Tag as GenericDictionary).Remove(node.Name);



                node.Remove();
            }
           
        }

        private static void Btn_Add_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var cmb = (btn.Parent as Form).Controls[Cmb_ADD]; 
            var node = btn.Tag as TreeNode;
            if (typeof(List<IAlgorithmClass>).IsAssignableFrom(node.Tag.GetType()))
            {
                var algoType = Assembly.GetAssembly(typeof(IAlgorithmClass)).GetTypes().Where(t => {
                    return t.Name == cmb.Text;
                }).ToArray();

                if (algoType.Length != 1)
                    return;

                var inspParam = node.Parent.Tag as GenericInspectoinPointParams;

                var algoInstance = Activator.CreateInstance(algoType[0]) as IAlgorithmClass;

                inspParam.AddAlgorithm(ref algoInstance);


                node.Parent.m_MappingTreeNodes((node.Parent.Tag as GenericInspectionParam).m_GetTreeNode(node.Parent.Name, Enum_TreeNodeParamsType.StorableAndUnstorable));


                //node.Nodes.Add(algoInstance.AlgoKeyString, algoInstance.AlgorithmName + " $" + algoInstance.AlgoKeyString);
                //node.Nodes[algoInstance.AlgoKeyString].Tag = algoInstance;

                //var storableAlgoParamNode = node.Parent.Nodes["StorableParams"]?.Nodes["Algorithm"]?.Nodes[algoInstance.AlgorithmName];
                //var unstorableAlgoParamNode = node.Parent.Nodes["UnstorableParams"]?.Nodes["Algorithm"]?.Nodes[algoInstance.AlgorithmName];

                //if(storableAlgoParamNode == null)
                //{

                //}


                //if(storableAlgoParamNode != null)
                //{
                //    var storableSubParamsDict = (storableAlgoParamNode.Tag as GenericDictionary)[algoInstance.AlgoKeyString].GetValue_TupleDict();
                //    var storabLSubParamsNode = storableSubParamsDict.m_GetTreeNode(algoInstance.AlgoKeyString);
                //    storabLSubParamsNode.Tag = storableSubParamsDict;
                //    storableAlgoParamNode.Nodes.Add(storabLSubParamsNode);


                //}
                //if(unstorableAlgoParamNode != null)
                //{
                //    var unstorableSubParamsDict = (unstorableAlgoParamNode.Tag as GenericDictionary)[algoInstance.AlgoKeyString].GetValue_TupleDict();
                //    var unstorabLSubParamsNode = unstorableSubParamsDict.m_GetTreeNode(algoInstance.AlgoKeyString);
                //    unstorabLSubParamsNode.Tag = unstorableSubParamsDict;
                //    unstorableAlgoParamNode.Nodes.Add(unstorabLSubParamsNode);
                //}

            }
            else if (typeof(List<GenericInspectionParam>).IsAssignableFrom(node.Tag.GetType()))
            {
                var inspParamsType = Assembly.GetAssembly(typeof(GenericInspectionParam)).GetTypes().Where(t => {
                    return t.Name == cmb.Text;
                }).ToArray();

                if (inspParamsType.Length != 1)
                    return;

                var inspParamsList = node.Tag as List<GenericInspectionParam>;
                var inspParamInstance = Activator.CreateInstance(inspParamsType[0]) as GenericInspectionParam;
                inspParamsList.Add(inspParamInstance);
                var recipe = node.Parent.Tag as RecipeClass;
                var srcNodes = recipe.GetRecipeTreeNode(Enum_TreeNodeParamsType.StorableAndUnstorable);

                node.Parent.m_MappingTreeNodes(srcNodes);
            }
            else if (typeof(GenericDictionary).IsAssignableFrom(node.Tag.GetType()))
            {
                if (cmb.Text == String.Empty)
                    return;
                 const string randomkeysample = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                    //string randomKey = randomkeysample.RandomString(7);
                var tempKey = cmb.Text + randomkeysample.RandomString(7);
                var dict = (node.Tag as GenericDictionary);
               while(dict.ContainsKey(tempKey))
                    tempKey = cmb.Text +"_"+ randomkeysample.RandomString(7);
               
               if(Enum.TryParse<Enum_InspTupleType>(cmb.Text,out var res))
                {
                    var gValue = new GenericTuple(res);
                    dict[tempKey] = gValue;
                    var newNode = gValue.m_GetTreeNode(tempKey);
                    node.Nodes.Add(newNode);

                }

              
                
            }

        }

        private static void TreeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            var roadMap = m_getNodeRoadmap(e.Node);

            if (!m_checkNodeEditable(roadMap))
                e.CancelEdit = true;




        }

        private static void m_setGenericRecipeControl(Form form,TreeNode node )
        {
            //////////////////////////////
            /// not complete.
           /// throw new NotImplementedException();
            
            var btn_add = form.Controls[Btn_ADD] as Button;
            var btn_remove = form.Controls[Btn_REMOVE] as Button;
            var cmb_add = form.Controls[Cmb_ADD] as ComboBox;
            var btn_sortup = form.Controls[Btn_SORTUP] as Button;
            var btn_sortdown = form.Controls[Btn_SORTDOWN] as Button;


            btn_add.Enabled = false;
            btn_remove.Enabled = false;
            cmb_add.Enabled = false;
            cmb_add.Items.Clear();
            cmb_add.Text = String.Empty;
            cmb_add.DropDownStyle = ComboBoxStyle.DropDownList;
            btn_sortdown.Enabled = false;
            btn_sortup.Enabled = false;




            cmb_add.Tag = node;
            btn_remove.Tag = node;
            btn_add.Tag = node;
            btn_sortup.Tag = node;
            btn_sortdown.Tag = node;

          

            //dict
            if (node.Tag.GetType() == typeof(GenericDictionary) )//Appendable dict
            {
                btn_add.Enabled = true;
                cmb_add.Enabled = true;

                if (!CannotEditNodes.Contains(node.Name)) // When true means node is top-directory (Storable or Unstorable dicts).
                {
                    btn_remove.Enabled = true;
                }

                var tupletypes = Enum.GetNames(typeof(Enum_InspTupleType));

                foreach(var tupletype in tupletypes)
                {
                    if(tupletype != "Bitmap")
                        cmb_add.Items.Add(tupletype);
                }

                cmb_add.SelectedIndex = 0;

            }

            //tuple
            if(node.Tag.GetType() == typeof(GenericTuple))
            {
                btn_remove.Enabled = true;

                cmb_add.Text = ((GenericTuple)node.Tag).TupleType.ToString();

            }
            //&& 

        
            // node.Tag.GetType().IsAssignableFrom(typeof(IAlgorithmClass[]))
            // algorithms of list
            if(node.Tag.GetType().IsAssignableFrom(typeof(List<IAlgorithmClass>)))// Algorithm list appendable 
            {
                btn_add.Enabled = true;
                cmb_add.Enabled = true;


                var algoTypes = m_GetAllChildClasses(typeof(IAlgorithmClass));



                foreach (var t in algoTypes)
                {
                    cmb_add.Items.Add(t.Name);
                }    
                  

                cmb_add.SelectedIndex = 0;


            }

            // insp-params of list
            if (node.Tag.GetType().IsAssignableFrom(typeof(List<GenericInspectionParam>)))// GenericInspectionParams list appendable 
            {
                btn_add.Enabled = true;
                cmb_add.Enabled = true;


                var types = m_GetAllAssignableClasses(typeof(GenericInspectionParam));



                foreach (var t in types)
                {
                    cmb_add.Items.Add(t.Name);
                }


                cmb_add.SelectedIndex = 0;


            }

            // algo
            if(typeof(IAlgorithmClass).IsAssignableFrom(node.Tag.GetType()))
            {
                btn_remove.Enabled = true;

                if (node.Index != node.Parent.Nodes.Count - 1)
                    btn_sortdown.Enabled = true;

                if(node.Index != 0)
                    btn_sortup.Enabled = true;


            }   
            
            //insp-param
            if(typeof(GenericInspectionParam).IsAssignableFrom(node.Tag.GetType()) && ! CannotEditNodes.Contains(node.Name))
            {
                btn_remove.Enabled = true;
               
            }
            
            


        }


        private static void formSizeChange(object sender, EventArgs e)
        {
            var form = sender as Form;
            var treeview = form.Controls[Trv_RecipeTreeview];
            var btn_save = form.Controls[Btn_SAVE];
            var btn_add = form.Controls[Btn_ADD];
            var btn_remove = form.Controls[Btn_REMOVE];
            var cmb_add = form.Controls[Cmb_ADD];

            var bound = (sender as Form).Size;

            var btn_sortUp = form.Controls[Btn_SORTUP];
            var btn_sortDown = form.Controls[Btn_SORTDOWN];


            if(treeview != null)
                treeview.Size = new Size((int)(bound.Width * 0.7), (int)(bound.Height * 0.9));

            if(btn_save != null)
            {
                btn_save.Size = new Size((int)(bound.Width * 0.1), (int)(bound.Height * 0.1));
                btn_save.Location = new Point(bound.Width - 50 - btn_save.Size.Width, bound.Height - 50 - btn_save.Height);
            }

            if(btn_add != null)
            {
                btn_add.Size = new Size((int)(bound.Width * 0.07), (int)(bound.Height * 0.1));
                btn_add.Location = new Point(bound.Width - 50 - btn_add.Size.Width, 0 + 10 );

                if (cmb_add != null)
                {
                    cmb_add.Size = new Size((int)(bound.Width * 0.20) - 20, btn_add.Height);
                    cmb_add.Location =  new Point(btn_add.Location.X - 10 - cmb_add.Size.Width , btn_add.Location.Y);

                    if(btn_sortUp != null)
                    {
                        btn_sortUp.Size = new Size((int)(bound.Width * 0.03), (int)(bound.Height  *0.03));
                        btn_sortUp.Location = new Point(cmb_add.Location.X, cmb_add.Location.Y + 20 + cmb_add.Height);

                        if (btn_sortDown != null)
                        {
                            btn_sortDown.Size = new Size((int)(bound.Width * 0.03), (int)(bound.Height * 0.03));
                            btn_sortDown.Location = new Point(btn_sortUp.Location.X, btn_sortUp.Location.Y + 20 + btn_sortUp.Height);
                        }
                    }
                }

                if (btn_remove != null)
                {
                    btn_remove.Size = new Size((int)(bound.Width * 0.07), (int)(bound.Height * 0.1));
                    btn_remove.Location = new Point(bound.Width - 50 - btn_remove.Size.Width, btn_add.Location.Y + btn_add.Height +  50);
                
                }
            }




       
        }
        private static void Btn_save_Click(object sender, EventArgs e)
        {
            var form = (sender as Button).Parent as Form;


            form.Close();

            form.DialogResult = DialogResult.OK;
        }

        private static void TreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label == null)
                return;
            if (e.Label == String.Empty)
            {
                var key = e.Node.Name;
                var nodeText = string.Empty;

                if (e.Node.Tag.GetType() == typeof(GenericTuple))
                    nodeText = ((GenericTuple)e.Node.Tag).m_GetTreeNode(key).Text;
                else if (e.Node.Tag.GetType() == typeof(GenericDictionary))
                    nodeText = ((GenericDictionary)e.Node.Tag).m_GetTreeNode(key).Text;

                e.CancelEdit = true;

                e.Node.Text = nodeText;



                e.Node.BackColor = Color_Normal;
                e.Node.TreeView.SelectedNode = null;
                return;


            }


            GenericDictionary lastDict = m_Nodes_GetLastGenericDictionaryParent(e.Node);

            if (lastDict == null)
                return;
            //this means dict rename
            if (e.Node.Tag.GetType() == typeof(GenericDictionary))
            {
                var old_key = e.Node.Name;
                var new_key = e.Label;


                if (lastDict.ContainsKey(new_key))
                {
                    var key = e.Node.Name;
                    var nodeText = string.Empty;

                    if (e.Node.Tag.GetType() == typeof(GenericTuple))
                        nodeText = ((GenericTuple)e.Node.Tag).m_GetTreeNode(key).Text;
                    else if (e.Node.Tag.GetType() == typeof(GenericDictionary))
                        nodeText = ((GenericDictionary)e.Node.Tag).m_GetTreeNode(key).Text;

                    e.CancelEdit = true;

                    e.Node.Text = nodeText;



                    e.Node.BackColor = Color_Normal;
                    e.Node.TreeView.SelectedNode = null;
                    return;
                }


                var dict = e.Node.Tag as GenericDictionary;
                lastDict.Remove(old_key);
                lastDict[new_key] = new GenericTuple(dict);
                e.Node.Name = new_key;
                e.Node.BackColor = Color_Changed;
            }
            else//run into here means node is not GenericDictionary , maybe rename the key or change value
            {


                var labelStrs = e.Label.Split(':');

                if (labelStrs.Length < 2) // label text is illegal
                {
                    e.Node.BackColor = Color_EditError;
                    e.Node.TreeView.SelectedNode = null;
                    return;
                }



                var old_key = e.Node.Name;
                var new_key = labelStrs[0];
                var value = labelStrs[1];
                var type = (e.Node.Tag as GenericTuple).TupleType;


                if (old_key != new_key)
                {

                    //Check key duplicated
                    if (lastDict.ContainsKey(new_key))
                    {
                        var key = e.Node.Name;
                        var nodeText = string.Empty;

                        if (e.Node.Tag.GetType() == typeof(GenericTuple))
                            nodeText = ((GenericTuple)e.Node.Tag).m_GetTreeNode(key).Text;
                        else if (e.Node.Tag.GetType() == typeof(GenericDictionary))
                            nodeText = ((GenericDictionary)e.Node.Tag).m_GetTreeNode(key).Text;

                        e.CancelEdit = true;

                        e.Node.Text = nodeText;



                        e.Node.BackColor = Color_Normal;
                        e.Node.TreeView.SelectedNode = null;
                        return;
                    }
                  }

                    ///Value type check ..
                    ///
                    var typematch = m_GenericTupleStringValueTypeCheck(type, value);

                if (!typematch)
                {
                    e.Node.BackColor = Color_EditTypeError;
                    e.Node.TreeView.SelectedNode = null;
                    return;
                }
                var retValueSet = m_GenericTupleSetValueByCastFromString(lastDict, new_key, type, value);
                if (!retValueSet)
                {
                    e.Node.BackColor = Color_EditTypeError;
                    e.Node.TreeView.SelectedNode = null;
                    return;
                }

                if (old_key != new_key)
                {

                    

                    lastDict.Remove(old_key);
                }



                e.Node.Tag = lastDict[new_key];
                e.Node.BackColor = Color_Changed;

            }

            e.Node.TreeView.SelectedNode = null;
        }


        private static void Btn_sortDown_Click(object sender, EventArgs e)
        {
           var node = (sender as Button).Tag as TreeNode;

           


            var algoList = node.Parent.Tag as List<IAlgorithmClass>;
            var algo = node.Tag as IAlgorithmClass;
            var currentIndex = algoList.IndexOf(algo);
            algoList.Remove(algo);
            algoList.Insert(currentIndex+1, algo);

   


            node.MoveDown();

            node.TreeView.SelectedNode = node;
        }

        private static void Btn_sortUp_Click(object sender, EventArgs e)
        {
            var node = (sender as Button).Tag as TreeNode;

            var algoList = node.Parent.Tag as List<IAlgorithmClass>;
            var algo = node.Tag as IAlgorithmClass;
            var currentIndex = algoList.IndexOf(algo);
            algoList.Remove(algo);
            algoList.Insert(currentIndex - 1, algo);

            node.MoveUp();

            node.TreeView.SelectedNode = node;
        }
        #endregion



    }
    public static class RecipeClassExtensions
    {
   

        public static object GetInspValue(this GenericDictionary dict, string key)
        {

            return dict[key].GetValue();
        }

        public static GenericTuple GetInspParam(this GenericDictionary dict, string key)
        {
            return dict[key];

        }


       
    }



    public class RecipeClass : ICloneable
    {

        private string _recipeName;
        private string _recipeNo;
        private string _recipeType;
        private string _recipeDescription;
        private string _recipeVersion;
        private int _recipeSequence;
        private List<GenericInspectionParam> _inspctionParams = new List<GenericInspectionParam>();
        private GenericInspectionParam _recipeParams = new GenericInspectoinRecipeParams();
        public string RecipeName { get { return _recipeName; } set { _recipeName = value; } }
        public string RecipeNo { get { return _recipeNo; } set { _recipeNo = value; } }
        public string RecipeType { get { return _recipeType; } set { _recipeType = value; } }
        public string RecipeDescription { get { return _recipeDescription; } set { _recipeDescription = value; } }
        public string RecipeVersion { get { return _recipeVersion; } set { _recipeVersion = value; } }
        public int RecipeSequence { get { return _recipeSequence; } set { _recipeSequence = value; } }
        public List<GenericInspectionParam> InspectionParams { get { return _inspctionParams; } set { _inspctionParams = value; } }
        public GenericInspectionParam RecipeParams { get { return _recipeParams; } set { _recipeParams = value; } }
        public RecipeClass()
        {

        }
        public RecipeClass(string RecipeName, String RecipeNo, string RecipeType, string RecipeDescription, string RecipeVersion, int RecipeSequence)
        {
            _recipeName = RecipeName;
            _recipeNo = RecipeNo;
            _recipeType = RecipeType;
            _recipeDescription = RecipeDescription;
            _recipeVersion = RecipeVersion;
            _recipeSequence = RecipeSequence;
        }


        public bool SaveRecipe(string FilePath)
        {
            var tempRCP = (RecipeClass)this.Clone();


            FileInfo f = new FileInfo(FilePath);
            f.Directory.Create();
            using (TextWriter tw = File.CreateText(f.FullName))
            {
                JsonSerializer jsonSerializer = new JsonSerializer();
                jsonSerializer.Formatting = Formatting.Indented;
                jsonSerializer.TypeNameHandling = TypeNameHandling.All;
                jsonSerializer.Serialize(tw, tempRCP);

            }
            return true;
        }


        public static bool ReadRecipe(string FilePath, out RecipeClass ReadRecipe)
        {
            ReadRecipe = null;
            FileInfo f = new FileInfo(FilePath);
            if (f.Exists == false)
                return false;

            using (TextReader tr = File.OpenText(f.FullName))
            {
                JsonSerializer js = new JsonSerializer();
                js.TypeNameHandling = TypeNameHandling.All;

                JsonReader jr = new JsonTextReader(tr);
                ReadRecipe = js.Deserialize<RecipeClass>(jr);
            }

            foreach (GenericInspectionParam p in ReadRecipe.InspectionParams)
            {
                GenericDictionary.JsonChildDictionarySerializeSupport(p.StorableParams);

                if (p.GetType() == typeof(GenericInspectoinPointParams))
                {


                    GenericInspectoinPointParams point = (GenericInspectoinPointParams)p;


                    foreach (var algo in point.AlgorithmList)
                    {
                        point.m_SetAlgorithmSubParamsDict(algo);
                        point.m_CreateAlgorithmSubParamsDict_IfNotExists(algo, GenericInspectionParam.Enum_ParamsType.Unstorable);
                        point.m_CreateAlgorithmSubParamsDict_IfNotExists(algo, GenericInspectionParam.Enum_ParamsType.Storable);
                        algo.Initialize(point);
                    }
                }
            }



            return true;
        }



        public object IClone()
        {
            return (RecipeClass)this.MemberwiseClone();
        }

        public object Clone()
        {
            var tempRecipe = (RecipeClass)this.MemberwiseClone();
            tempRecipe.RecipeType = String.Copy(this.RecipeType);
            tempRecipe.RecipeName = String.Copy(this.RecipeName);
            tempRecipe.RecipeNo = String.Copy(this.RecipeNo);
            tempRecipe.RecipeType = String.Copy(this.RecipeType);
            tempRecipe.RecipeVersion = String.Copy(this.RecipeVersion);
            tempRecipe._inspctionParams = new List<GenericInspectionParam>();
            tempRecipe.RecipeParams = (GenericInspectionParam)this.RecipeParams.Clone();
            foreach (GenericInspectionParam p in this.InspectionParams)
            {
                var tempPoint = (GenericInspectionParam)p.Clone();

                tempRecipe.InspectionParams.Add(tempPoint);

            }



            return tempRecipe;

        }


        public enum Enum_TreeNodeParamsType
        {
            StorableAndUnstorable,
            Storable,
            Unstorable
        }


        
  }




    /// <summary>
    /// 
    /// </summary>
    public class GenericInspectionParam :  ICloneable
    {
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.Initialize();

        }

        protected virtual GenericDictionary m_StorableParamsSet()
        {
            return new GenericDictionary();
        }
        protected virtual GenericDictionary m_UnstorableParamsSet()
        {
            return new GenericDictionary();
        }
        public virtual bool Initialize()
        {
            var storableDict = this.StorableParams;
            var unstorableDict = this.UnstorableParams;


            if (storableDict != null)
                GenericDictionary.MappingClone(m_StorableParamsSet(), storableDict);
            if (unstorableDict != null)
                GenericDictionary.MappingClone(m_UnstorableParamsSet(), unstorableDict);

            if (storableDict == null || unstorableDict == null)
                return false;

            return true;
        }

        public enum Enum_ParamsType
        {
            Storable,
            Unstorable
        }
        public object Clone()
        {
            GenericInspectionParam temp = (GenericInspectionParam)this.MemberwiseClone();
            temp.StorableParams = GenericDictionary.CloneWithoutTempParameters(this.StorableParams);
            temp.UnstorableParams = GenericDictionary.CloneWithoutTempParameters(this.UnstorableParams);
            

            return temp;
        }






        private GenericDictionary _storableParams = new GenericDictionary();
        private GenericDictionary _unstorableResults = new GenericDictionary();

        public GenericDictionary StorableParams { get { return _storableParams; } set { _storableParams = value; } }

        [JsonIgnore]
        public GenericDictionary UnstorableParams { get { return _unstorableResults; } set { _unstorableResults = value; } }

        public GenericInspectionParam()
        {

            this.Initialize();
        }


        private void m_AppendStorableAndUnstorableNode(TreeNode node)
        {
            node.Nodes.Add("StorableParams", "StorableParams");
            node.Nodes.Add("UnstorableParams", "UnstorableParams");

        }

       
        internal virtual TreeNode m_GetTreeNode(string nodeName, Enum_TreeNodeParamsType treenodeType)
        {
            var node = new TreeNode();
            node.Name = nodeName;
            node.Text = nodeName;
            node.Tag = this;
            //m_AppendStorableAndUnstorableNode(node);


            if(treenodeType == Enum_TreeNodeParamsType.StorableAndUnstorable || treenodeType == Enum_TreeNodeParamsType.Storable)
            {
                node.Nodes.Add(this.StorableParams.m_GetTreeNode("StorableParams"));


            }
            if (treenodeType == Enum_TreeNodeParamsType.StorableAndUnstorable || treenodeType == Enum_TreeNodeParamsType.Unstorable)
            {
                node.Nodes.Add(this.UnstorableParams.m_GetTreeNode("UnstorableParams"));
            }


            return node;
        }

    }






}

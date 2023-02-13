using GenericRecipe.Algorithm;
using GenericRecipe.GenericType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GenericRecipe.RecipeClass;

namespace GenericRecipe
{
    /// <summary>
    /// ParamDict => 算法集 位於 Storable/Unstorable Params=> Algorithm => AlgorithmName.Get_TupleDict()
    /// SubParamDict=> 算法子集 位於 Storable/Unstorable ParamDict => AlgorithmObjectKey.Get_TupleDict()
    /// AlgorithmDict => 回傳 Storable/Unstoable Params => Algorithm.Get_TupleDict()
    /// </summary>
    public class GenericInspectoinPointParams : GenericInspectionParam
    {
        internal override TreeNode m_GetTreeNode(string nodename, Enum_TreeNodeParamsType treenodeType)
        {
            var node = base.m_GetTreeNode(nodename, treenodeType);
            node.Nodes.Add("Algorithm", "Algorithm");
            node.Nodes["Algorithm"].Tag = this.AlgorithmList;
            //
            foreach (var algo in this.AlgorithmList)
            {
                node.Nodes["Algorithm"].Nodes.Add(algo.AlgoKeyString, algo.AlgorithmName + " $" + algo.AlgoKeyString);
                node.Nodes["Algorithm"].Nodes[algo.AlgoKeyString].Tag = algo;

            }

            return node;
        }

        private const String Point = "Point";
        private const String X = "X";
        private const String Y = "Y";
        private const String Z = "Z";
        private const string Algorithm = "Algorithm";
        private List<IAlgorithmClass> _algorithmList = new List<IAlgorithmClass>();
        public List<IAlgorithmClass> AlgorithmList { get { return _algorithmList; } private set { _algorithmList = value; } }

        #region algorithm


        private bool m_CheckAlgoDictExists(Enum_ParamsType ptype)
        {
            if (
               !this.m_GetParameters(ptype).ContainsKey(Algorithm) ||
               this.m_GetParameters(ptype)[Algorithm].GetValueType() != typeof(GenericDictionary))
                return false;

            return true;
        }

        private bool m_CheckAlgoParamsDictExists<IAlgo>(Enum_ParamsType ptype) where IAlgo : IAlgorithmClass
        {

            var dict = m_GetAlgorithmDict(ptype);
            if (dict == null)
                return false;

            if (!dict.ContainsKey(typeof(IAlgo).Name) ||
                dict[typeof(IAlgo).Name].GetValueType() != typeof(GenericDictionary))
                return false;


            return true;
        }
        private bool m_CheckAlgoParamsDictExists(IAlgorithmClass algo, Enum_ParamsType ptype)
        {

            var dict = m_GetAlgorithmDict(ptype);
            if (dict == null)
                return false;
            if (!dict.ContainsKey(algo.AlgorithmName) ||
                dict[algo.AlgorithmName].GetValueType() != typeof(GenericDictionary))
                return false;


            return true;
        }


        private bool m_CheckAlgoSubParamDictExists(IAlgorithmClass algo, Enum_ParamsType ptype)
        {
            var dict = m_GetAlgorithmParamsDict(algo, ptype);

            if (dict == null ||
                !dict.ContainsKey(algo.AlgoKeyString) ||
                dict[algo.AlgoKeyString].GetValueType() != typeof(GenericDictionary)
                )
                return false;

            return true;
        }
        private GenericDictionary m_GetAlgorithmDict(Enum_ParamsType ptype)
        {
            if (m_CheckAlgoDictExists(ptype) == false)
                return null;

            return this.m_GetParameters(ptype)[Algorithm].GetValue_TupleDict();

        }

        private GenericDictionary m_GetAlgorithmParamsDict<IAlgo>(Enum_ParamsType ptype) where IAlgo : IAlgorithmClass
        {

            var dict = m_GetAlgorithmDict(ptype);

            if (dict == null)
                return null;

            if (
                !m_CheckAlgoParamsDictExists<IAlgo>(ptype)
                )
                return null;

            GenericDictionary algoDict = dict[typeof(IAlgo).Name].GetValue_TupleDict();


            return algoDict;

        }
        private GenericDictionary m_GetAlgorithmParamsDict(IAlgorithmClass algo, Enum_ParamsType ptype)
        {
            if (!m_CheckAlgoParamsDictExists(algo, ptype))
                return null;

            var dict = m_GetAlgorithmDict(ptype);

            if (dict == null)
                return null;



            GenericDictionary algoDict = dict[algo.AlgorithmName].GetValue_TupleDict();


            return algoDict;

        }



        public GenericDictionary GetAlgorithmSubParamsDict(IAlgorithmClass algo, Enum_ParamsType ptype)
        {


            if (!m_CheckAlgoSubParamDictExists(algo, ptype))
                return null;
            var paramDict = m_GetAlgorithmParamsDict(algo, ptype);






            return paramDict[algo.AlgoKeyString].GetValue_TupleDict();

        }
        private GenericDictionary m_GetParameters(Enum_ParamsType ptype)
        {
            if (ptype == Enum_ParamsType.Storable)
                return this.StorableParams;
            else
                return this.UnstorableParams;
        }

        internal bool m_CreateAlgorithmSubParamsDict_IfNotExists(IAlgorithmClass algo, Enum_ParamsType pytpe)
        {
            GenericDictionary paramDict = this.m_GetAlgorithmParamsDict(algo, pytpe);

            if (!paramDict.ContainsKey(algo.AlgoKeyString))
            {
                paramDict.AddEmptyDictionary(algo.AlgoKeyString);
            }
            else
                return false;
            return true;
        }
        private bool m_CreateAlgorithmParamDict_IfNotExists(IAlgorithmClass algo, Enum_ParamsType ptype)
        {
            var dict = m_GetAlgorithmDict(ptype);
            if (dict == null)
                return false;

            if (!m_CheckAlgoParamsDictExists(algo, ptype))
                dict.AddEmptyDictionary(algo.AlgorithmName);

            return true;
        }
        private bool m_CreateAlgorithmParamDict_IfNotExists<IAlgo>(Enum_ParamsType ptype) where IAlgo : IAlgorithmClass
        {
            var dict = m_GetAlgorithmDict(ptype);
            if (dict == null)
                return false;

            if (!m_CheckAlgoParamsDictExists<IAlgo>(ptype))
                dict.AddEmptyDictionary(typeof(IAlgo).Name);

            return true;
        }

        internal bool m_SetAlgorithmSubParamsDict(IAlgorithmClass algo)
        {
            m_CreateAlgorithmParamDict_IfNotExists(algo, Enum_ParamsType.Storable);
            m_CreateAlgorithmParamDict_IfNotExists(algo, Enum_ParamsType.Unstorable);
            GenericDictionary paramDictStorable = this.m_GetAlgorithmParamsDict(algo, Enum_ParamsType.Storable);
            GenericDictionary paramDictUnstoable = this.m_GetAlgorithmParamsDict(algo, Enum_ParamsType.Unstorable);

            m_CreateAlgorithmSubParamsDict_IfNotExists(algo, Enum_ParamsType.Storable);
            m_CreateAlgorithmSubParamsDict_IfNotExists(algo, Enum_ParamsType.Unstorable);
            return true;
        }

        //public bool AddAlgorithm<IAlgo>() where IAlgo:IAlgorithmClass
        //{

        //    m_CreateAlgorithmParamDict_IfNotExists<IAlgo>(Enum_ParamsType.Storable);
        //    m_CreateAlgorithmParamDict_IfNotExists<IAlgo>(Enum_ParamsType.Unstorable);


        //    const string randomkeysample = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        //    string randomKey = randomkeysample.RandomString(7);

        //    GenericDictionary paramDictStorable = this.m_GetAlgorithmParamsDict<IAlgo>(Enum_ParamsType.Storable);
        //    GenericDictionary paramDictUnstoable = this.m_GetAlgorithmParamsDict<IAlgo>(Enum_ParamsType.Unstorable);

        //    //if (paramDictStorable == null|| paramDictUnstoable == null)
        //    //    return false;


        //    ///Check is key exists
        //    for(int i = 0; i<= 1000; i++)
        //    {
        //        if (paramDictStorable.ContainsKey(randomKey)|| paramDictUnstoable.ContainsKey(randomKey))
        //            randomKey = randomkeysample.RandomString(5);
        //        else
        //            break;
        //    }


        //    if (paramDictStorable.ContainsKey(randomKey)|| paramDictUnstoable.ContainsKey(randomKey))
        //        return false; //Try too mach times so left this cycle.

        //    var algo = (IAlgo)Activator.CreateInstance(typeof(IAlgo));



        //    algo.AlgoKeyString = randomKey;
        //    this.AlgorithmList.Add(algo);

        //    m_SetAlgorithmSubParamsDict(algo);
        //    ///////Append params to the dict
        //    algo.Initialize(this);
        //    ///////


        //    return true;
        //}
        public bool AddAlgorithm<IAlgo>() where IAlgo : IAlgorithmClass
        {
            var algo = (IAlgorithmClass)Activator.CreateInstance(typeof(IAlgo));
            return this.AddAlgorithm(ref algo);
        }
        public bool AddAlgorithm(ref IAlgorithmClass algorithm)
        {
            if (this.AlgorithmList.Contains(algorithm))
                return false;

            m_CreateAlgorithmParamDict_IfNotExists(algorithm, Enum_ParamsType.Storable);
            m_CreateAlgorithmParamDict_IfNotExists(algorithm, Enum_ParamsType.Unstorable);


            const string randomkeysample = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string randomKey = randomkeysample.RandomString(7);

            GenericDictionary paramDictStorable = this.m_GetAlgorithmParamsDict(algorithm, Enum_ParamsType.Storable);
            GenericDictionary paramDictUnstoable = this.m_GetAlgorithmParamsDict(algorithm, Enum_ParamsType.Unstorable);

            //if (paramDictStorable == null|| paramDictUnstoable == null)
            //    return false;


            ///Check is key exists
            for (int i = 0; i <= 1000; i++)
            {
                if (paramDictStorable.ContainsKey(randomKey) || paramDictUnstoable.ContainsKey(randomKey))
                    randomKey = randomkeysample.RandomString(5);
                else
                    break;
            }


            if (paramDictStorable.ContainsKey(randomKey) || paramDictUnstoable.ContainsKey(randomKey))
                return false; //Try too mach times so left this cycle.





            algorithm.AlgoKeyString = randomKey;
            this.AlgorithmList.Add(algorithm);

            m_SetAlgorithmSubParamsDict(algorithm);
            ///////Append params to the dict
            algorithm.Initialize(this);
            ///////


            return true;
        }
        public bool RemoveAlgorithm(IAlgorithmClass algo)
        {
            var dict = m_GetAlgorithmParamsDict(algo, Enum_ParamsType.Storable);
            var unDict = m_GetAlgorithmParamsDict(algo, Enum_ParamsType.Unstorable);

            if (dict != null || unDict != null)
            {
                algo.Remove(this);



                if (dict != null)
                    dict.Remove(algo.AlgoKeyString);

                if (unDict != null)
                    unDict.Remove(algo.AlgoKeyString);

                this.AlgorithmList.Remove(algo);

            }////////////////////////////////////////////here
            else
                return false;


            algo.Dispose(this);
            return true;


        }

        public bool Algorithm_Execute()
        {
            return false;
        }
        #endregion
        public GenericInspectoinPointParams()
        {

            Initialize();

        }
        protected override GenericDictionary m_StorableParamsSet()
        {
            GenericDictionary ret = new GenericDictionary();
            ret.Add(Point, new GenericTuple(
             new GenericDictionary(
                 new KeyValuePair<string, GenericTuple>[]{
                    new KeyValuePair<string,GenericTuple>(X, new GenericTuple(0.0F)),
                    new KeyValuePair<string,GenericTuple>(Y, new GenericTuple(0.0F)),
                    new KeyValuePair<string,GenericTuple>(Z, new GenericTuple(0.0F)),

                 }
                 )
             ));
            ret.Add("Light", new GenericTuple(100));
            ret.AddEmptyDictionary(Algorithm);

            return ret;
        }
        protected override GenericDictionary m_UnstorableParamsSet()
        {
            GenericDictionary ret = new GenericDictionary();
            ret.AddEmptyDictionary(Algorithm);
            return ret;
        }

    }


    public class GenericInspectoinRecipeParams : GenericInspectionParam
    {


        private const string GlassInfo = "GlassInfo";

        private List<IAlgorithmClass> _algorithmList = new List<IAlgorithmClass>();
        public List<IAlgorithmClass> AlgorithmList { get { return _algorithmList; } private set { _algorithmList = value; } }


        public GenericInspectoinRecipeParams() : base()
        {

            Initialize();

        }
        protected override GenericDictionary m_UnstorableParamsSet()
        {
            GenericDictionary ret = new GenericDictionary();
            ret.Add(GlassInfo, new GenericTuple(
             new GenericDictionary(
                 new KeyValuePair<string, GenericTuple>[]{
                    new KeyValuePair<string,GenericTuple>("GlassID", new GenericTuple(string.Empty)),
                    new KeyValuePair<string,GenericTuple>("GlassType", new GenericTuple(string.Empty)),
                    new KeyValuePair<string,GenericTuple>("Judgement", new GenericTuple(0))
                 }
                 )
             ));
            ret.Add("FolderPath", new GenericTuple(string.Empty));


            return ret;
        }
        protected override GenericDictionary m_StorableParamsSet()
        {
            GenericDictionary ret = new GenericDictionary();

            return ret;
        }

    }
}

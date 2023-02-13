using GenericRecipe.GenericType;
using System.Collections.Generic;
namespace GenericRecipe.Algorithm
{


    public abstract class IAlgorithmClass 
    {

        protected virtual GenericDictionary m_StorableParamsSet()
        {
             return new GenericDictionary();
        }
        protected virtual GenericDictionary m_UnstorableParamsSet()
        {
            return new GenericDictionary();
        }
        public virtual bool Initialize(GenericInspectoinPointParams param)
        {
            var storableDict = param.GetAlgorithmSubParamsDict(this, GenericInspectoinPointParams.Enum_ParamsType.Storable);
            var unstorableDict = param.GetAlgorithmSubParamsDict(this, GenericInspectionParam.Enum_ParamsType.Unstorable);


            if (storableDict != null)
                GenericDictionary.MappingClone(m_StorableParamsSet(), storableDict);
            if (unstorableDict != null)
                GenericDictionary.MappingClone(m_UnstorableParamsSet(), unstorableDict);

            if (storableDict == null || unstorableDict == null)
                return false;

            return true;
        }

        public IAlgorithmClass()
        {
                
        }
        public  string AlgoKeyString { get; set; }
        public  string AlgorithmName { get { return this.GetType().Name; } }
        public  abstract bool Excute( GenericInspectoinPointParams param);
       

        public virtual bool Remove( GenericInspectoinPointParams param)
        {
            return true;
        }
        public abstract bool Dispose( GenericInspectoinPointParams param);

    }
  


}

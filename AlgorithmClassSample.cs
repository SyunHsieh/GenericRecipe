using GenericRecipe.Algorithm;
using GenericRecipe.GenericType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericRecipe
{


    public class Alignment_TwoSumAlgorithm : IAlgorithmClass
    {
        protected override GenericDictionary m_StorableParamsSet()
        {
            return new GenericDictionary(
                new KeyValuePair<string, GenericTuple>[]
                {
                    new KeyValuePair<string, GenericTuple>("A",new GenericTuple(50)),
                    new KeyValuePair<string, GenericTuple>("B",new GenericTuple(100)),
               
                }

                );

        }
        protected override GenericDictionary m_UnstorableParamsSet()
        {
            return new GenericDictionary(
                new KeyValuePair<string, GenericTuple>[]
                {
                    //new KeyValuePair<string, GenericTuple>("X",new GenericTuple(0.0F)),
                    //new KeyValuePair<string, GenericTuple>("Y",new GenericTuple(0.0F)),

                }

                );
        }
        public Alignment_TwoSumAlgorithm()
        {

        }

        public override bool Dispose(GenericInspectoinPointParams param)
        {
            return true;
        }

        public override bool Excute(GenericInspectoinPointParams param)
        {
            int ret = param.StorableParams["A"].GetValue_Int16() + param.StorableParams["B"].GetValue_Int16();
            return true;
        }




    }





    public class Alignment_CrossMarkAlgorithm : IAlgorithmClass
    {
        protected override GenericDictionary m_StorableParamsSet()
        {
            return new GenericDictionary(
                new KeyValuePair<string, GenericTuple>[]
                {
                    new KeyValuePair<string, GenericTuple>("GrayMin",new GenericTuple(50)),
                    new KeyValuePair<string, GenericTuple>("GrayMax",new GenericTuple(100)),
                    new KeyValuePair<string, GenericTuple>("ROI",new GenericTuple(
                        new GenericDictionary(
                            new KeyValuePair<string, GenericTuple>[]
                            {
                                new KeyValuePair<string, GenericTuple>("X1",new GenericTuple(1237.356F)),
                                new KeyValuePair<string, GenericTuple>("Y1",new GenericTuple(1230.356F)),
                                new KeyValuePair<string, GenericTuple>("X2",new GenericTuple(223.356F)),
                                new KeyValuePair<string, GenericTuple>("Y2",new GenericTuple(2230.356F))
                            })
                        )),
                    new KeyValuePair<string , GenericTuple>("MarkType" , new GenericTuple("Cross")),
                    new KeyValuePair<string , GenericTuple>("MarkDict2",new GenericTuple("D:\\123.xld"))
                }

                );

        }
        protected override GenericDictionary m_UnstorableParamsSet()
        {
            return new GenericDictionary(
                new KeyValuePair<string, GenericTuple>[]
                {
                    new KeyValuePair<string, GenericTuple>("X",new GenericTuple(0.0F)),
                    new KeyValuePair<string, GenericTuple>("Y",new GenericTuple(0.0F)),

                }

                );
        }
        public Alignment_CrossMarkAlgorithm()
        {

        }

        public override bool Dispose(GenericInspectoinPointParams param)
        {
            return true;
        }

        public override bool Excute(GenericInspectoinPointParams param)
        {
            return true;
        }




    }
}

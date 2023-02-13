using System;
using System.Collections.Generic;
using System.Drawing;
using static GenericRecipe.GenericType.GenericTuple;
using System.Windows.Forms;
namespace GenericRecipe.GenericType
{


    public class GenericTuple : ICloneable
    {
        public virtual GenericTuple this[string key]
        {
            get
            {
                return this.GetValue_TupleDict()[key];
            }

        }
        internal virtual TreeNode m_GetTreeNode(string nodename)
        {
            var node = new TreeNode();
            node.Name = nodename;
            node.Text = nodename;
            node.Tag = this;
            switch (this.TupleType)
            {
                case Enum_InspTupleType.GenericDictionary:
                    node = this.GetValue_TupleDict().m_GetTreeNode(nodename);
                    break;
                default:
                    node.Text = nodename + ": " + this.ParamValue.ToString();
                    break;
            }
            return node;
        }
        public enum Enum_InspTupleType
        {
            Int16,
            Int32,
            Int64,
            Single,
            Double,
            String,
            GenericDictionary,
            Bitmap
        }


        private Enum_InspTupleType? _tupleType;
        private object _paramValue;

        public Enum_InspTupleType? TupleType { get { return _tupleType; } set { _tupleType = value; } }
        public object ParamValue { get { return _paramValue; } set { _paramValue = value; }}

        //public Enum_InspTupleTpye? TupleType { get => _tupleType; set => _tupleType = value; }
        //public object ParamValue { get => _paramValue; set => _paramValue = value; }
        private GenericTuple()
        {
            
        }

        public Type GetValueType()
        {
            return this.GetValue().GetType();
        }
        public GenericTuple(Enum_InspTupleType type)
        {

            TupleType = type;
            switch (type)
            {
                case Enum_InspTupleType.Int16:
                    Int16 v= 0;
                    ParamValue = v;
                    break;
                case Enum_InspTupleType.Int32:
                    Int32 v2 = 0;
                    ParamValue = v2;
                    break;
                case Enum_InspTupleType.Int64:
                    Int64 v3 = 0;
                    ParamValue = v3;
                    break;
                case Enum_InspTupleType.Single:
                    Single v4 = 0.0f;
                    ParamValue = v4;
                    break;
                case Enum_InspTupleType.Double:
                    Double v5 = 0.0f;
                    ParamValue = v5;
                    break;
                case Enum_InspTupleType.String:
                    ParamValue = string.Empty;
                    break;
                case Enum_InspTupleType.GenericDictionary:
                    ParamValue = new GenericDictionary();
                    break;
                case Enum_InspTupleType.Bitmap:
                    ParamValue = new Bitmap(10,10);
                    break;
            }
        }
        public GenericTuple(object value)
        {
            var temp = value.GetType();
            Enum_InspTupleType tempEnum = new Enum_InspTupleType();


            if (temp == typeof(Int16))
            {
                tempEnum = Enum_InspTupleType.Int16;
            }
            else if (temp == typeof(Int32))
            {
                tempEnum = Enum_InspTupleType.Int32;
            }
            else if (temp == typeof(Int64))
            {
                tempEnum = Enum_InspTupleType.Int64;
            }
            else if (temp == typeof(Single))
            {
                tempEnum = Enum_InspTupleType.Single;
            }
            else if (temp == typeof(Double))
            {
                tempEnum = Enum_InspTupleType.Double;
            }
            else if (temp == typeof(String))
            {
                tempEnum = Enum_InspTupleType.String;
            }
            else if (temp == typeof(GenericDictionary))
            {
                tempEnum = Enum_InspTupleType.GenericDictionary;
            }
            else if (temp == typeof(Bitmap))
            {
                tempEnum = Enum_InspTupleType.Bitmap;
            }


            this.ParamValue = value;
            this.TupleType = tempEnum;




        }
        ////public GenericTuple(object value, Enum_InspTupleType tupleType)
        ////{
        ////    ParamValue = value;
        ////    TupleType = tupleType;
        ////}
        public GenericTuple(Enum_InspTupleType tupleType, object value)
        {
            ParamValue = value;
            TupleType = tupleType;
        }

        public void SetParamValue(object value, Enum_InspTupleType tupleType)
        {
            ParamValue = value;
            TupleType = tupleType;
        }
        public void SetParamValue(Enum_InspTupleType tupleType, object value)
        {
            ParamValue = value;
            TupleType = tupleType;
        }

        public Bitmap GetValue_Bitmap()
        {
            return (Bitmap)ParamValue;
        }
        public Int16 GetValue_Int16()
        {
            return Convert.ToInt16(ParamValue);
        }
        public Int32 GetValue_Int32()
        {
            return Convert.ToInt32(ParamValue);
        }
        public Int64 GetValue_Int64()
        {
            return Convert.ToInt64(ParamValue);
        }
        public Single GetValue_Single()
        {
            return Convert.ToSingle(ParamValue);
        }
        public Double GetValue_Double()
        {
            return Convert.ToDouble(ParamValue);
        }
        public string GetValue_String()
        {
            return Convert.ToString(ParamValue);
        }
        public GenericDictionary GetValue_TupleDict()
        {
            return (GenericDictionary)this.ParamValue;
        }


        public object GetValue()
        {
            if (this.TupleType == null)
                return null;

            switch (this.TupleType)
            {
                case Enum_InspTupleType.Int16:
                    return Convert.ToInt16(ParamValue);
                case Enum_InspTupleType.Int32:
                    return Convert.ToInt32(ParamValue);
                case Enum_InspTupleType.Int64:
                    return Convert.ToInt64(ParamValue);
                case Enum_InspTupleType.Single:
                    return Convert.ToSingle(ParamValue);
                case Enum_InspTupleType.Double:
                    return Convert.ToDouble(ParamValue);
                case Enum_InspTupleType.String:
                    return Convert.ToString(ParamValue);
                case Enum_InspTupleType.GenericDictionary:
                    return (GenericDictionary)this.ParamValue;
                case Enum_InspTupleType.Bitmap:
                    return (Bitmap)ParamValue;
                default:
                    throw new Exception("Unknow type of class SetParamValue.GetValue Type name : " + this.TupleType.ToString());

            }


        }

        public object Clone()
        {

            GenericTuple tempTuple;
            switch (this.TupleType)
            {
                case Enum_InspTupleType.Bitmap:
                    tempTuple = new GenericTuple(Enum_InspTupleType.Bitmap, this.ParamValue);
                    break;
                case Enum_InspTupleType.GenericDictionary:
                    tempTuple = new GenericTuple(Enum_InspTupleType.GenericDictionary, GenericDictionary.DeepClone(this.GetValue_TupleDict()));
                    break;
                case Enum_InspTupleType.String:
                    tempTuple = new GenericTuple(Enum_InspTupleType.String, string.Copy(this.GetValue_String()));
                    break;
                default:
                    tempTuple = (GenericTuple)this.MemberwiseClone();
                    break;
            }


            return tempTuple;
        }
    }

    public class GenericDictionary : Dictionary<string, GenericTuple>
    {
        internal virtual TreeNode m_GetTreeNode(string nodename)
        {
            var node = new TreeNode();
            node.Name = nodename;
            node.Text = nodename;
            node.Tag = this;
           foreach(KeyValuePair<string, GenericTuple> kvp in this)
            {
                node.Nodes.Add(kvp.Value.m_GetTreeNode(kvp.Key));
            }
            return node;
        }
        public GenericDictionary() : base() { }

        public void AddEmptyDictionary(string key)
        {
            this[key] = new GenericTuple(new GenericDictionary());

        }

        public GenericDictionary(KeyValuePair<string, GenericTuple>[] keypair) : base()
        {
            foreach (var pair in keypair)
            {
                this[pair.Key] = pair.Value;
            }
        }
        public bool Add_WhenKeyNotExists(string key, GenericTuple value)
        {
            bool ret = true;

            if (!this.ContainsKey(key))
                this[key] = value;
            else
                ret = false;

            return ret;
        }
        public enum Enum_CloneType
        {
            NoOverwrite,
            Overwrite,
            DifferentTypeOverwriteOnly,

        }
        public static bool MappingClone(GenericDictionary sourceDict, GenericDictionary destDict, Enum_CloneType cloneType = Enum_CloneType.DifferentTypeOverwriteOnly)
        {
            if (sourceDict == null || destDict == null || sourceDict == destDict)
                return false;

            foreach (KeyValuePair<string, GenericTuple> pair in sourceDict)
            {
                var needClone = false;

                if (!destDict.ContainsKey(pair.Key))
                {
                    needClone = true;
                }
                else
                {
                    switch (cloneType)
                    {
                        case Enum_CloneType.Overwrite:
                            needClone = true;
                            break;
                        case Enum_CloneType.DifferentTypeOverwriteOnly:
                            if (pair.Value.TupleType != destDict[pair.Key].TupleType)
                                needClone = true;
                            break;

                    }
                }

                if (needClone)
                    destDict[pair.Key] = (GenericTuple)pair.Value.Clone();



            }

            return true;
        }


        public static void JsonChildDictionarySerializeSupport(GenericDictionary dict)
        {
            foreach (KeyValuePair<string, GenericTuple> p in dict)
            {

                switch (p.Value.TupleType)
                {

                    case Enum_InspTupleType.GenericDictionary:
                        //p.Value.ParamValue = JsonConvert.DeserializeObject<GenericDictionary>(p.Value.ParamValue.ToString());

                        JsonChildDictionarySerializeSupport(p.Value.GetValue_TupleDict());
                        break;
                    default:
                        continue;
                }

            }
        }

        public static GenericDictionary DeepClone(GenericDictionary dict)
        {
            var retDict = new GenericDictionary();

            foreach (KeyValuePair<string, GenericTuple> pair in dict)
            {
                retDict[pair.Key] = (GenericTuple)pair.Value.Clone();
            }
            return retDict;
        }
        public static GenericDictionary CloneWithoutTempParameters(GenericDictionary dict)
        {
            GenericDictionary ret = new GenericDictionary();
            foreach (KeyValuePair<string, GenericTuple> p in dict)
            {
                GenericTuple tempP;
                switch (p.Value.TupleType)
                {
                    case Enum_InspTupleType.Bitmap:
                        continue;
                    case Enum_InspTupleType.GenericDictionary:
                        tempP = new GenericTuple(Enum_InspTupleType.GenericDictionary, CloneWithoutTempParameters(((GenericTuple)p.Value).GetValue_TupleDict()));
                        break;
                    case Enum_InspTupleType.String:
                        tempP = new GenericTuple(Enum_InspTupleType.String, string.Copy(p.Value.GetValue_String()));
                        break;
                    default:
                        tempP = (GenericTuple)p.Value.Clone();
                        break;
                }
                ret[p.Key] = tempP;
            }


            return ret;
        }
    }




}

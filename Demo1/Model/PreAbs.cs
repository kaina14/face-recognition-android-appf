using System;
using System.Collections.Generic;
using System.Text;

namespace Demo1.Model
{
    public class PreAbs
    {
        public Tuple<bool, string> kaina { get; set; } = new Tuple<bool, string>(false,nameof(kaina));
        public Tuple<bool, string> alfiya { get; set; } = new Tuple<bool, string>(false, nameof(alfiya));
        public Tuple<bool, string> Vaishnavi { get; set; }= new Tuple<bool, string>(false,nameof(Vaishnavi));



        public object this [string name]
        {
            get
            {
                if (name.ToUpper() == "KAINA")
                {
                    return kaina;
                }
                else if (name.ToUpper() == "ALFIYA")
                {
                    return alfiya;
                }
                else if (name.ToUpper() == "VAISHNAVI")
                {
                    return Vaishnavi;
                }
                else
                    return null;
            }
            set
            {
                if(name.ToUpper()=="KAINA")
                {
                    kaina = new Tuple<bool, string>((bool)value,nameof(kaina).ToLower());
                }
                else if(name.ToUpper()=="ALFIYA")
                {
                    alfiya = new Tuple<bool, string>((bool)value, nameof(alfiya).ToLower());
                }
                else if(name.ToUpper() =="VAISHNAVI")
                {
                    Vaishnavi = new Tuple<bool, string>((bool)value, nameof(Vaishnavi).ToLower());
                }
            }
            
        }


        public object this[int nameIndex]
        {
            get
            {
                if (nameIndex == 0)
                {
                    return kaina;
                }
                else if (nameIndex == 1)
                {
                    return alfiya;
                }
                else if (nameIndex == 2)
                {
                    return Vaishnavi;
                }
                else
                    return null;
            }
            set
            {
                if (nameIndex == 0)
                {
                    kaina = new Tuple<bool, string>((bool)value,nameof(kaina).ToLower());
                }
                else if (nameIndex == 1)
                {
                    alfiya = new Tuple<bool, string>((bool)value, nameof(alfiya).ToLower());
                }
                else if (nameIndex == 2)
                {
                    Vaishnavi = new Tuple<bool, string>((bool)value, nameof(Vaishnavi).ToLower());
                }
            }

        }
    }
}

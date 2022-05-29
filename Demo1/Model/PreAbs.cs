using System;
using System.Collections.Generic;
using System.Text;

namespace Demo1.Model
{
    public class PreAbs
    {
        public Tuple<bool, string> hrishikesh { get; set; } = new Tuple<bool, string>(false,nameof(hrishikesh));
        public Tuple<bool, string> ram { get; set; } = new Tuple<bool, string>(false, nameof(ram));
        public Tuple<bool, string> omkar { get; set; }= new Tuple<bool, string>(false,nameof(omkar));



        public object this [string name]
        {
            get
            {
                if (name.ToUpper() == "HRISHIKESH")
                {
                    return hrishikesh;
                }
                else if (name.ToUpper() == "RAM")
                {
                    return ram;
                }
                else if (name.ToUpper() == "OMKAR")
                {
                    return omkar;
                }
                else
                    return null;
            }
            set
            {
                if(name.ToUpper()=="HRISHIKESH")
                {
                    hrishikesh = new Tuple<bool, string>((bool)value,nameof(hrishikesh).ToLower());
                }
                else if(name.ToUpper()=="RAM")
                {
                    ram = new Tuple<bool, string>((bool)value, nameof(hrishikesh).ToLower());
                }
                else if(name.ToUpper() =="OMKAR")
                {
                    omkar = new Tuple<bool, string>((bool)value, nameof(hrishikesh).ToLower());
                }
            }
            
        }


        public object this[int nameIndex]
        {
            get
            {
                if (nameIndex == 0)
                {
                    return hrishikesh;
                }
                else if (nameIndex == 1)
                {
                    return ram;
                }
                else if (nameIndex == 2)
                {
                    return omkar;
                }
                else
                    return null;
            }
            set
            {
                if (nameIndex == 0)
                {
                    hrishikesh = new Tuple<bool, string>((bool)value,nameof(hrishikesh).ToLower());
                }
                else if (nameIndex == 1)
                {
                    ram = new Tuple<bool, string>((bool)value, nameof(ram).ToLower());
                }
                else if (nameIndex == 2)
                {
                    omkar = new Tuple<bool, string>((bool)value, nameof(omkar).ToLower());
                }
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace license
{
    //public class licenseModules
    //{
    //    #region Properties
    //    private List<licenseModules> oModuleList = null;
    //    public int Id { get; set; }     
    //    public bool HasAccess { get; set; }
    //    #endregion

    //    #region ctor
    //    public licenseModules()
    //    {
    //        oModuleList = new List<licenseModules>();
    //        oModuleList.Add(new licenseModules(1, true));
    //        oModuleList.Add(new licenseModules(2, true));
    //        oModuleList.Add(new licenseModules(3, true));
    //        oModuleList.Add(new licenseModules(4, true));
    //        //string text = "", DomainName = "", currentDomainName = "";
    //        //try
    //        //{
    //        //    currentDomainName = HttpContext.Current.Request.Url.Host.ToLower();
    //        //    if (currentDomainName.StartsWith("www."))//Domain
    //        //    {
    //        //        currentDomainName =  currentDomainName.Remove(currentDomainName.IndexOf("www."),4);
    //        //    }
    //        //    string lisencefile = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).AbsolutePath.ToLower().Replace("license.dll", "license.lic");
    //        //    if (System.IO.File.Exists(lisencefile))
    //        //    {
    //        //        text = CoreLib.Infrastructure.Cryptography.StringCipher.Decrypt(System.IO.File.ReadAllText(lisencefile), currentDomainName);
    //        //        string[] LicenseList = text.Trim().Split('+');
    //        //        DomainName = LicenseList.Skip(LicenseList.Count() - 1).First().ToLower().Trim();             

    //        //        if (currentDomainName == DomainName.ToLower())
    //        //        {
    //        //            foreach (string item in LicenseList.Take(LicenseList.Count() - 1))
    //        //            {
    //        //                int[] row = item.Split(',').Select(int.Parse).ToArray();
    //        //                switch (row[0])
    //        //                {
    //        //                    case 1: oModuleList.Add(new licenseModules(1, Convert.ToBoolean(row[1]))); break;
    //        //                    case 2: oModuleList.Add(new licenseModules(2, Convert.ToBoolean(row[1]))); break;
    //        //                    case 3: oModuleList.Add(new licenseModules(3, Convert.ToBoolean(row[1]))); break;
    //        //                    case 4: oModuleList.Add(new licenseModules(4, Convert.ToBoolean(row[1]))); break;
    //        //                }

    //        //            }
    //        //        }
    //        //        else//متاسفانه شما قوانین مربوط به خرید لایسنس نرم افزار را رعایت نکرده و نمی توانید از  آن استفاده کنید
    //        //            oModuleList.Add(new licenseModules(-1,false));
    //        //    }
    //        //    else
    //        //    {
    //        //        oModuleList.Add(new licenseModules(-1, false));
    //        //    }
    //        //}
    //        //catch (Exception)
    //        //{
    //        //    oModuleList.Add(new licenseModules(-1, false));
    //        //}

    //    }

    //    public licenseModules(int id, bool hasaccess)
    //    {
    //        this.Id = id;
    //        this.HasAccess = hasaccess;
    //    }
    //    #endregion

    //    #region Methods
    //    public List<licenseModules> GetlicenseModules()
    //    {
    //        return oModuleList;
    //    }
    //    #endregion
    //}
}

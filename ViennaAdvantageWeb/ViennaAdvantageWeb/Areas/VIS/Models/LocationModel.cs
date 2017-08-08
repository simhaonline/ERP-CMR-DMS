﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class LocationModel
    {
        public int C_LOCATION_ID { get; set; }
        public int C_City_ID { get; set; }
        public int C_Country_ID { get; set; }
        public int C_Region_ID { get; set; }

        [Display(Name = "Search")]
        public List<SearchAddress> Addresslist { get; set; }
        public Dictionary<int, string> countryList { get; set; }

        [Display(Name = "test")]
        public string Address1 { get; set; }
        [Display(Name = "test")]
        public string Address2 { get; set; }
        [Display(Name = "test")]
        public string Address3 { get; set; }
        [Display(Name = "test")]
        public string Address4 { get; set; }
        [Display(Name = "test")]
        public string Country { get; set; }
        [Display(Name = "test")]
        public string City { get; set; }
        [Display(Name = "test")]
        public string RegionName { get; set; }
        [Display(Name = "test")]
        public string Postal_Add { get; set; }
        [Display(Name = "test")]
        public string Postal { get; set; }
        [Display(Name = "test")]
        public string ZipCode { get; set; }

        /// <summary>
        /// addres get from database
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public LocationModel GetAddressfromDataBase(string locationId)
        {
            LocationModel obj = new LocationModel();
            //string sql = "select l.ADDRESS1,l.ADDRESS2,l.ADDRESS3,l.ADDRESS4,l.CITY,l.REGIONNAME ,l.POSTAL,l.POSTAL_ADD,cn.Name as Country,l.AD_CLIENT_ID,l.AD_ORG_ID,l.C_CITY_ID," +
            //             " l.C_COUNTRY_ID,l.C_LOCATION_ID,l.C_REGION_IDfrom c_location l--left join c_city ct on ct.c_city_id=l.c_city_id--left join c_region s on s.c_region_id=l.C_REGION_ID" +
            //             " left join c_country cn on cn.C_COUNTRY_ID=l.C_COUNTRY_ID where l.ISACTIVE='Y'";

            string sql = "SELECT L.ADDRESS1,L.ADDRESS2,L.ADDRESS3,L.ADDRESS4,L.CITY,L.REGIONNAME ,L.POSTAL,L.POSTAL_ADD,CN.NAME AS COUNTRY,L.AD_CLIENT_ID,L.AD_ORG_ID,L.C_CITY_ID," +
                        " L.C_COUNTRY_ID,L.C_LOCATION_ID,L.C_REGION_ID FROM C_LOCATION L" +
                        " LEFT JOIN C_COUNTRY CN ON CN.C_COUNTRY_ID=L.C_COUNTRY_ID ";

            var ds = new DataSet();
            try
            {
                ds = DB.ExecuteDataset(sql);

                if (ds != null)
                {
                    DataRow[] dr = ds.Tables[0].Select("c_location_id=" + locationId);
                    //if contain saved records
                    if (dr.Length > 0)
                    {
                        if (dr[0].ItemArray.Length > 0)
                        {
                            obj.Address1 = Convert.ToString(dr[0]["Address1"] == DBNull.Value ? "" : dr[0]["Address1"]);
                            obj.Address2 = Convert.ToString(dr[0]["Address2"] == DBNull.Value ? "" : dr[0]["Address2"]);
                            obj.Address3 = Convert.ToString(dr[0]["Address3"] == DBNull.Value ? "" : dr[0]["Address3"]);
                            obj.Address4 = Convert.ToString(dr[0]["Address4"] == DBNull.Value ? "" : dr[0]["Address4"]);

                            obj.C_City_ID = Convert.ToInt32(dr[0]["C_City_ID"] == DBNull.Value ? 0 : dr[0]["C_City_ID"]);
                            obj.C_Country_ID = Convert.ToInt32(dr[0]["C_Country_ID"] == DBNull.Value ? 0 : dr[0]["C_Country_ID"]);
                            obj.C_Region_ID = Convert.ToInt32(dr[0]["C_Region_ID"] == DBNull.Value ? 0 : dr[0]["C_Region_ID"]);

                            obj.City = Convert.ToString(dr[0]["City"] == DBNull.Value ? "" : dr[0]["City"]);
                            obj.RegionName = Convert.ToString(dr[0]["RegionName"] == DBNull.Value ? "" : dr[0]["RegionName"]);
                            obj.Country = Convert.ToString(dr[0]["Country"] == DBNull.Value ? "" : dr[0]["Country"]);

                            obj.ZipCode = Convert.ToString(dr[0]["POSTAL"] == DBNull.Value ? "" : dr[0]["POSTAL"]);
                        }
                    }

                    ////Auto fill search control
                    //obj.Addresslist = new List<SearchAddress>();
                    //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    //{
                    //    SearchAddress searchObj = new SearchAddress();
                    //    if (i == 0)
                    //    {
                    //        obj.Addresslist.Add(searchObj);
                    //        searchObj = new SearchAddress();
                    //    }
                    //    searchObj.Name = Convert.ToString(ds.Tables[0].Rows[i]["Country"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["Country"]) +
                    //        "," + Convert.ToString(ds.Tables[0].Rows[i]["Address1"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["Address1"])
                    //        + "," + Convert.ToString(ds.Tables[0].Rows[i]["Address2"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["Address2"])
                    //        + "," + Convert.ToString(ds.Tables[0].Rows[i]["Address3"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["Address3"])
                    //        + "," + Convert.ToString(ds.Tables[0].Rows[i]["Address4"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["Address4"])
                    //        + "," + Convert.ToString(ds.Tables[0].Rows[i]["City"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["City"])
                    //        + "," + Convert.ToString(ds.Tables[0].Rows[i]["RegionName"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["RegionName"])
                    //        + "," + Convert.ToString(ds.Tables[0].Rows[i]["POSTAL"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["POSTAL"]);

                    //    searchObj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["c_location_id"] == DBNull.Value ? 0 : ds.Tables[0].Rows[i]["c_location_id"]);
                    //    searchObj.CityId = Convert.ToInt32(ds.Tables[0].Rows[i]["C_City_ID"] == DBNull.Value ? 0 : ds.Tables[0].Rows[i]["C_City_ID"]);
                    //    searchObj.StateId = Convert.ToInt32(ds.Tables[0].Rows[i]["C_Region_ID"] == DBNull.Value ? 0 : ds.Tables[0].Rows[i]["C_Region_ID"]);
                    //    searchObj.CountryId = Convert.ToInt32(ds.Tables[0].Rows[i]["C_Country_ID"] == DBNull.Value ? 0 : ds.Tables[0].Rows[i]["C_Country_ID"]);

                    //    obj.Addresslist.Add(searchObj);
                    //}
                }
            }
            catch (Exception ev)
            {

            }
            ds = null;
            return obj;
        }

        /// <summary>
        /// save location
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="pref"></param>
        /// <returns></returns>
        public MLocation LocationSave(Ctx ctx, Dictionary<string, object> pref)
        {
            var c_Location_Id = Convert.ToInt32(pref["clocationId"]);
            MLocation _location = new MLocation(ctx, c_Location_Id, null);
            _location.SetAddress1(Convert.ToString(pref["addvalue1"]));
            _location.SetAddress2(Convert.ToString(pref["addvalue2"]));
            _location.SetAddress3(Convert.ToString(pref["addvalue3"]));
            _location.SetAddress4(Convert.ToString(pref["addvalue4"]));
            _location.SetCity(Convert.ToString(pref["cityValue"]));
            _location.SetPostal(Convert.ToString(pref["zipValue"]));
            //  Country/Region
            _location.SetC_Country_ID(Convert.ToInt32(pref["countryId"]));

            if (_location.GetCountry().IsHasRegion())
            {
                _location.SetC_Region_ID(Convert.ToInt32(pref["stateId"]));
            }
            else
            {
                _location.SetC_Region_ID(0);
            }
            _location.SetRegionName(Convert.ToString(pref["stateValue"]));
            _location.SetC_City_ID(Convert.ToInt32(pref["cityId"]));

            _location.Save();
            return _location;
        }

        /// <summary>
        /// Country search
        /// </summary>
        /// <param name="name_startsWith"></param>
        /// <returns></returns>
        public List<KeyNamePair> GetCountryByText(string name_startsWith)
        {
            List<KeyNamePair> obj = new List<KeyNamePair>();
            string sqlquery = " select C_COUNTRY_ID,Name from c_country where LOWER(name) like LOWER('" + name_startsWith + "%')";
            var ds = new DataSet();
            ds = DB.ExecuteDataset(sqlquery);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj.Add(new KeyNamePair(Convert.ToInt32(ds.Tables[0].Rows[i]["C_COUNTRY_ID"]), Convert.ToString(ds.Tables[0].Rows[i]["Name"])));
                }
            }
            ds = null;
            return obj;
        }

        /// <summary>
        /// State search
        /// </summary>
        /// <param name="name_startsWith"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public List<KeyNamePair> GetStatesByText(string name_startsWith, string countryId)
        {
            List<KeyNamePair> obj = new List<KeyNamePair>();
            string sqlquery = " select C_REGION_id,Name from C_REGION where C_COUNTRY_ID=" + countryId + " and LOWER(name) like LOWER('" + name_startsWith + "%')";
            var ds = new DataSet();
            ds = DB.ExecuteDataset(sqlquery);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj.Add(new KeyNamePair(Convert.ToInt32(ds.Tables[0].Rows[i]["C_REGION_id"]), Convert.ToString(ds.Tables[0].Rows[i]["Name"])));
                }
            }
            ds = null;
            return obj;
        }

        /// <summary>
        /// Addres search
        /// </summary>
        /// <param name="name_startsWith"></param>
        /// <returns></returns>
        public List<LocationAddress> GetAddressesSearch(Ctx ctx, string name_startsWith)
        {
            List<LocationAddress> obj = new List<LocationAddress>();

            //string sqlquery = " SELECT * FROM (SELECT (NVL(cn.Name,'')||' '|| NVL(l.ADDRESS1,'') ||' '|| NVL(l.ADDRESS2,'') ||' '|| NVL(l.ADDRESS3,'') ||' '|| NVL(l.ADDRESS4,'') ||' '|| NVL(l.CITY,'') ||' '|| NVL(l.REGIONNAME,'') ||' '|| NVL(l.POSTAL,'') ||' '|| NVL(l.POSTAL_ADD,'')) as address," +
            //                      "cn.Name, l.ADDRESS1 , l.ADDRESS2 , l.ADDRESS3 , l.ADDRESS4 , l.CITY , l.REGIONNAME , l.POSTAL , l.POSTAL_ADD," +
            //                      " l.AD_CLIENT_ID,l.AD_ORG_ID,l.C_CITY_ID,l.C_COUNTRY_ID,l.C_LOCATION_ID,l.C_REGION_ID FROM C_Location l" +
            //                      " LEFT JOIN C_Country cn on cn.C_COUNTRY_ID=l.C_COUNTRY_ID WHERE l.ISACTIVE='Y') qb1" +
            //                      " WHERE LOWER(qb1.address) LIKE LOWER('" + name_startsWith + "%') or LOWER(qb1.address) LIKE LOWER('%" + name_startsWith + "%') or LOWER(qb1.address) LIKE LOWER('%" + name_startsWith + "')";

            string sqlquery = " SELECT * FROM (SELECT (NVL(cn.Name,'')||' '|| NVL(l.ADDRESS1,'') ||' '|| NVL(l.ADDRESS2,'') ||' '|| NVL(l.ADDRESS3,'') ||' '|| NVL(l.ADDRESS4,'') ||' '|| NVL(l.CITY,'') ||' '|| NVL(l.REGIONNAME,'') ||' '|| NVL(l.POSTAL,'') ||' '|| NVL(l.POSTAL_ADD,'')) as address," +
                                 "cn.Name, l.ADDRESS1 , l.ADDRESS2 , l.ADDRESS3 , l.ADDRESS4 , l.CITY , l.REGIONNAME , l.POSTAL , l.POSTAL_ADD," +
                                 " l.AD_CLIENT_ID,l.AD_ORG_ID,l.C_CITY_ID,l.C_COUNTRY_ID,l.C_LOCATION_ID,l.C_REGION_ID FROM C_Location l" +
                                 " LEFT JOIN C_Country cn on cn.C_COUNTRY_ID=l.C_COUNTRY_ID WHERE l.ISACTIVE='Y') qb1";
            sqlquery = MRole.GetDefault(ctx).AddAccessSQL(sqlquery, "C_Location", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);
            sqlquery = "SELECT * FROM (" + sqlquery + " ) fltr WHERE LOWER(fltr.address) LIKE LOWER('" + name_startsWith + "%') or LOWER(fltr.address) LIKE LOWER('%" + name_startsWith + "%') or LOWER(fltr.address) LIKE LOWER('%" + name_startsWith + "')";



            var ds = new DataSet();
            ds = DB.ExecuteDataset(sqlquery);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    LocationAddress ladd = new LocationAddress
                    {
                        ADDRESS = Convert.ToString(ds.Tables[0].Rows[i]["address"]),
                        COUNTRYNAME = Convert.ToString(ds.Tables[0].Rows[i]["Name"]),
                        ADDRESS1 = Convert.ToString(ds.Tables[0].Rows[i]["ADDRESS1"]),
                        ADDRESS2 = Convert.ToString(ds.Tables[0].Rows[i]["ADDRESS2"]),
                        ADDRESS3 = Convert.ToString(ds.Tables[0].Rows[i]["ADDRESS3"]),
                        ADDRESS4 = Convert.ToString(ds.Tables[0].Rows[i]["ADDRESS4"]),

                        CITYNAME = Convert.ToString(ds.Tables[0].Rows[i]["CITY"]),
                        STATENAME = Convert.ToString(ds.Tables[0].Rows[i]["REGIONNAME"]),
                        ZIPCODE = Convert.ToString(ds.Tables[0].Rows[i]["POSTAL"]),

                        AD_CLIENT_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_CLIENT_ID"]),
                        AD_ORG_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_ORG_ID"]),
                        // C_CITY_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["C_CITY_ID"]),
                        C_COUNTRY_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["C_COUNTRY_ID"] == DBNull.Value ? null : ds.Tables[0].Rows[i]["C_COUNTRY_ID"]),
                        C_LOCATION_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["C_LOCATION_ID"]),
                        C_REGION_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["C_REGION_ID"] == DBNull.Value ? null : ds.Tables[0].Rows[i]["C_REGION_ID"])
                    };
                    obj.Add(ladd);
                }
            }
            ds = null;
            return obj;
        }

        /// <summary>
        /// Get Location List
        /// </summary>
        /// <param name="ctx">client context</param>
        /// <returns>keyname pair dictionary</returns>
        internal static Dictionary<int, KeyNamePair> GetAllLocations(VAdvantage.Utility.Ctx ctx)
        {
            IDataReader dr = null;
            int MAX_ROWS = 10000;
            Dictionary<int, KeyNamePair> locs = new Dictionary<int, KeyNamePair>();
            try
            {
                dr = VAdvantage.DataBase.DB.ExecuteReader("SELECT * FROM C_Location");
                while (dr.Read())
                {
                    if (MAX_ROWS > 10000)
                        break;
                    MLocation loc = MLocation.Get(ctx, Convert.ToInt32(dr["C_Location_ID"]), null);
                    locs[loc.Get_ID()] = new KeyNamePair(loc.Get_ID(), loc.ToString());
                }
                dr.Close();
            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }

            }
            return locs;
        }


    }

    public class SearchAddress
    {
        public String Name
        {
            get;
            set;
        }

        public int ID
        {
            get;
            set;
        }
        public int CityId
        {
            get;
            set;
        }
        public int StateId
        {
            get;
            set;
        }
        public int CountryId
        {
            get;
            set;
        }
    }

    public class LocationAddress
    {
        public string ADDRESS { get; set; }

        public string ADDRESS1 { get; set; }
        public string ADDRESS2 { get; set; }
        public string ADDRESS3 { get; set; }
        public string ADDRESS4 { get; set; }

        public string COUNTRYNAME { get; set; }
        public string CITYNAME { get; set; }
        public string STATENAME { get; set; }
        public string ZIPCODE { get; set; }

        public int AD_CLIENT_ID { get; set; }
        public int AD_ORG_ID { get; set; }
        // public int? C_CITY_ID { get; set; }
        public int? C_COUNTRY_ID { get; set; }
        public int C_LOCATION_ID { get; set; }
        public int? C_REGION_ID { get; set; }
    }
}
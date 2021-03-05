using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using MFaaP.MFWSClient;
using Newtonsoft.Json;

namespace GameVaultApp.Data
{

    public class MFilesAPI
    {
        public static MFilesAPI instance;
        string baseUrl;
        MFWSClient client;

        public enum Class
        {
            Document = 0,
            ProductDocument = 5,
            Device = 6,
            Platform = 7,
            Periphery = 8,
            Program = 9,
            Videogame = 10
        }
        #region OldCode (not using MFAAP.MFWSClient library)
        /*
        RestClient client;
        public MFilesAPI(string _userName, string _password, string _vaultGUID, string _baseUrl)
        {
            baseUrl = _baseUrl;
            client = new RestClient(baseUrl);
            Authenticate(_userName, _password, _vaultGUID);
            if (instance != null)
            {
                instance = this;
            }
        }

       
        private ExtendedObjectVersion[] GetObjectsByParameters(string parameter = "", bool include = false)
        {

            string includeString = "";
            if (include)
            {
                includeString = "&include=properties";
            }
            string url = string.Format("/objects?{0}{1}", parameter, includeString);
            RestRequest request = new RestRequest(url, Method.GET);
            IRestResponse response = client.Execute(request);
            var results = Newtonsoft.Json.JsonConvert.DeserializeObject<Results<ExtendedObjectVersion>>(response.Content);


            return results.Items.ToArray();

        }
        private void Authenticate(string username, string password, string GUID)
        {
            string authUrl = "/server/authenticationtokens";
            var authObject = new
            {
                Username = username,
                Password = password,
                VaultGUID = GUID
            };

            RestRequest request = new RestRequest(authUrl, Method.POST);
            request.AddJsonBody(authObject);

            IRestResponse response = client.Execute(request);

            dynamic jsonResponse = SimpleJson.DeserializeObject(response.Content);

            string token = jsonResponse.Value;

            client.AddDefaultHeader("X-Authentication", token);
            client.AddDefaultHeader("Content-Type", "application/json");
        }
        */
        #endregion
        public MFilesAPI(string _userName, string _password, string _vaultGUID, string _baseUrl)
        {
            baseUrl = _baseUrl;
            client = new MFWSClient(baseUrl);
            client.AuthenticateUsingCredentials(new Guid(_vaultGUID), _userName, _password);
            if (instance != null)
            {
                instance = this;
            }

        }
        public ObjectVersion[] GetAllObjects()
        {
            return client.ObjectSearchOperations.SearchForObjectsByConditions();
        }

        public ExtendedObjectVersion[] GetAllObjectsWithProperties(string objectTitle = null, int? objectTypeId = null, Class? objClass = null)
        {
            
            ObjectVersion[] objs = GetObjectsByParameters(objectTitle, objectTypeId, objClass);
            ExtendedObjectVersion[] extObjs = new ExtendedObjectVersion[objs.Count()];
            PropertyValue[][] allProps = GetAllPropertiesFromArray(objs);

            for (int i = 0; i < objs.Length; i++)
            {
                var serialized = JsonConvert.SerializeObject(objs[i]);
                extObjs[i] = JsonConvert.DeserializeObject<ExtendedObjectVersion>(serialized);
                extObjs[i].Properties = allProps[i].ToList();
            }

            return extObjs;
        }

    

        public PropertyValue[][] GetAllPropertiesFromArray(ObjectVersion[] objVersions)
        {
            ObjVer[] objVers = objVersions.Select(obj => obj.ObjVer).ToArray();
            return client.ObjectPropertyOperations.GetPropertiesOfMultipleObjects(objVers);
        }
        public PropertyValue[][] GetAllProperties()
        {
            ObjectVersion[] objVersions = GetAllObjects();
            ObjVer[] objVers = objVersions.Select(obj => obj.ObjVer).ToArray();
            return client.ObjectPropertyOperations.GetPropertiesOfMultipleObjects(objVers);
        }

        public string GetPropDefDisplay(PropertyValue propDefID)
        {
            PropertyDef propdev = client.PropertyDefOperations.GetPropertyDef(propDefID.PropertyDef);

            return propdev.Name;
        }
        public List<ObjType> GetAllObjectTypes()
        {
            return client.ObjectTypeOperations.GetObjectTypes();
        }

        public ObjectVersion[] GetObjectsByParameters(string objectTitle, int ? objectTypeId, Class? objClass)
        {
            List<ISearchCondition> conditions = new List<ISearchCondition>();
            if (objectTypeId != null)
            {
                ISearchCondition condition = new ObjectTypeSearchCondition((int)objectTypeId);
                conditions.Add(condition);
            }
            if (objectTitle != null)
            {
                ISearchCondition condition = new TextPropertyValueSearchCondition(0, objectTitle, SearchConditionOperators.Contains);
                conditions.Add(condition);
            }
            if (objClass != null)
            {
                ISearchCondition condition = new NumericPropertyValueSearchCondition(100, (int)objClass);
                conditions.Add(condition);
            }

            return client.ObjectSearchOperations.SearchForObjectsByConditions(conditions.ToArray());
        }

    }
}

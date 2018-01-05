using System;
using System.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qx.EntitySerialization
{
    public class EntityFileMetaData
    {
        private string _description;
        private string _type;
        private DateTime _creationTime;

        private const int DESCRIPTION_MAX_LENGTH = 20;
        public const string CREATION_DATE_FORMAT = "yyyyMMddHHmm";
        internal const string FILE_EXT_NAME = ".qxdb";
        private const string NEXT_VERSION_SETTINGS_KEY = "NextVersion";
        

        public EntityFileMetaData(string type, string description)
        {
            _type = type;
            if (description.Length > DESCRIPTION_MAX_LENGTH)
                description = description.Substring(0, DESCRIPTION_MAX_LENGTH - 1);
            _description = description.Replace(".", String.Empty).Replace(" ", "_");
            _creationTime = DateTime.UtcNow;
            Version = GetNextVersion();
        }

        public int Version { get; set; }

        public DateTime CreationTime { get { return _creationTime; } }

        private int GetNextVersion()
        {
            var nextVersion = int.Parse(SerializationConfiguration.Get(SerializationConfiguration.ConfigKey.NextVersion));
            SerializationConfiguration.Set(SerializationConfiguration.ConfigKey.NextVersion, (nextVersion + 1).ToString());
            return nextVersion;
        }

        public string GenerateFileName()
        {
            return $"{Version}_{_type}_{_description}_{_creationTime.ToString(CREATION_DATE_FORMAT)}{FILE_EXT_NAME}";
        }

        public static EntityFileMetaData FromFileName(string fileName)
        {
            fileName = fileName.Substring(0, fileName.LastIndexOf(FILE_EXT_NAME));
            var attributes = fileName.Split('_');
            var fileMetaData = new EntityFileMetaData(attributes[1], attributes[2]);
            fileMetaData._creationTime = DateTime.ParseExact(attributes[3], CREATION_DATE_FORMAT, CultureInfo.InvariantCulture);
            fileMetaData.Version = int.Parse(attributes[0]);
            return fileMetaData;
        }
    }
}

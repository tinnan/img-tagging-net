using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace img_tagging.tag.converter
{
    public class TagsConverter : JsonConverter
    {
        private const string F_SITES = "Sites";
        private const string F_ACT = "Actresses";
        private const string F_TAGLIST = "Taglist";
        private const string F_NAME = "Name";
        private const string F_DESC = "Description";
        private const string F_MEMBERS = "Members";

        public override bool CanConvert(Type objectType)
        {
            return typeof(Tags).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load object from stream.
            JObject jo = JObject.Load(reader);

            Tags tags = new Tags();

            // read Sites field.
            if(FieldExists(F_SITES, jo))
            {
                tags.Sites = new HashSet<string>(jo[F_SITES].ToObject<string[]>());
            }

            // read Actresses field.
            if(FieldExists(F_ACT, jo))
            {
                tags.Actresses = new HashSet<string>(jo[F_ACT].ToObject<string[]>());
            }
            
            // read Taglist field.
            if(FieldExists(F_TAGLIST, jo))
            {
                // get Taglist field.
                JToken token = jo.GetValue(F_TAGLIST);
                // convert it to list of tag object.
                tags.Taglist = token.ToObject<List<Tag>>();
            }
            
            return tags;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private bool FieldExists(string field, JObject jo)
        {
            return jo[field] != null;
        }
       
    }
}

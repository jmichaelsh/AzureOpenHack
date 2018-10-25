

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Backend_API_Minecraft.Model
{ 

    [DataContract]
    public partial class Server : IEquatable<Server>
    { 

        [DataMember(Name="name")]
        public string Name { get; set; }


        [DataMember(Name="endpoints")]
        public Endpoint Endpoints { get; set; }


        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Server {\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Endpoints: ").Append(Endpoints).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }


        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Server)obj);
        }


        public bool Equals(Server other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    Name == other.Name ||
                    Name != null &&
                    Name.Equals(other.Name)
                ) && 
                (
                    Endpoints == other.Endpoints ||
                    Endpoints != null &&
                    Endpoints.Equals(other.Endpoints)
                );
        }


        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                    if (Name != null)
                    hashCode = hashCode * 59 + Name.GetHashCode();
                    if (Endpoints != null)
                    hashCode = hashCode * 59 + Endpoints.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(Server left, Server right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Server left, Server right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}

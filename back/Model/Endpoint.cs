

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
    public partial class Endpoint : IEquatable<Endpoint>
    { 
       
        [DataMember(Name="minecraft")]
        public string Minecraft { get; set; }


        [DataMember(Name="rcon")]
        public string Rcon { get; set; }

       
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Endpoint {\n");
            sb.Append("  Minecraft: ").Append(Minecraft).Append("\n");
            sb.Append("  Rcon: ").Append(Rcon).Append("\n");
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
            return obj.GetType() == GetType() && Equals((Endpoint)obj);
        }

       
        public bool Equals(Endpoint other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    Minecraft == other.Minecraft ||
                    Minecraft != null &&
                    Minecraft.Equals(other.Minecraft)
                ) && 
                (
                    Rcon == other.Rcon ||
                    Rcon != null &&
                    Rcon.Equals(other.Rcon)
                );
        }

      
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                    if (Minecraft != null)
                    hashCode = hashCode * 59 + Minecraft.GetHashCode();
                    if (Rcon != null)
                    hashCode = hashCode * 59 + Rcon.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(Endpoint left, Endpoint right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Endpoint left, Endpoint right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}

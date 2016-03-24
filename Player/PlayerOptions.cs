using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Player.Properties;

using CommSub;

namespace Player
{
    public class PlayerOptions : RuntimeOptions
    {
        public override void SetDefaults()
        {
            if (string.IsNullOrWhiteSpace(Registry))
                Registry = Properties.Settings.Default.RegistryEndPoint;
            if (string.IsNullOrWhiteSpace(ANumber))
                ANumber = Properties.Settings.Default.ANumber;
            if (string.IsNullOrWhiteSpace(FirstName))
                FirstName = Properties.Settings.Default.FirstName;
            if (string.IsNullOrWhiteSpace(LastName))
                LastName = Properties.Settings.Default.LastName;
            if (string.IsNullOrWhiteSpace(Alias))
                Alias = Properties.Settings.Default.Alias;
            if (MinPortNullable == null)
                MinPortNullable = Properties.Settings.Default.MinPort;
            if (MaxPortNullable == null)
                MaxPortNullable = Properties.Settings.Default.MaxPort;
            if (TimeoutNullable == null)
                TimeoutNullable = Properties.Settings.Default.Timeout;
            if (RetriesNullable == null)
                RetriesNullable = Properties.Settings.Default.MaxRetries;
            if (string.IsNullOrWhiteSpace(Label))
                Label = string.Format("{0}'s Player", Alias);
        }
    }
}

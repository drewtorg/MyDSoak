﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using log4net;
using log4net.Config;
using CommandLine;

namespace BalloonStore
{
    static class Program
    {
        private static ILog Logger = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Logger.Info("Starting up");

            BalloonStoreOptions options = new BalloonStoreOptions();

            if (Parser.Default.ParseArguments(args, options))
            {
                options.SetDefaults();

                BalloonStore store = new BalloonStore(options);
                BalloonStoreForm form = new BalloonStoreForm(store);
                store.Start();
                Application.Run(form);
            }
            else
            {
                Logger.Info("Provided args: " + string.Join(" ", args));
                Logger.Info(options.GetUsage());
            }
        }
    }
}

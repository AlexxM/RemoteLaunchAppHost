/*
 * Сделано в SharpDevelop.
 * Пользователь: 055makarov
 * Дата: 16.10.2014
 * Время: 13:06
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;

namespace RemoteLaunchAppHost
{
	class Program
	{
		ServiceHost sh;
		
		public static void Main(string[] args)
		{
            StartService();
		}
		

		
		private static void StartService()
		{
            try
            {
                Console.WriteLine("Starting RemoteLaunchAppHost...");
                ServiceHost sh;
                string ip = GetServerIp();
                if (ip != null)
                {
                    sh = new ServiceHost(typeof(RemoteLaunchAppService), new Uri("net.tcp://" + ip + ":8089"));
                }
                else
                {
                    sh = new ServiceHost(typeof(RemoteLaunchAppService), new Uri("net.tcp://192.168.0.1:8089"));
                }

                sh.AddServiceEndpoint(typeof(IRemoteLaunchAppService), new NetTcpBinding(SecurityMode.None), "RemoteLaunchAppService");

                sh.Description.Behaviors.Add(new System.ServiceModel.Description.ServiceMetadataBehavior());
                sh.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
                Binding mexBinding = MetadataExchangeBindings.CreateMexTcpBinding();
                sh.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, "mex");
                sh.AddServiceEndpoint(new UdpDiscoveryEndpoint());
                sh.Open();
                Console.WriteLine("OK...");
                foreach (var i in sh.Description.Endpoints)
                {
                    Console.WriteLine("Adress: {0}, binding: {1}", i.Address, i.Binding);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadLine();
            }
		
		}
		
		
		
		public static string GetServerIp()
		{
			IPHostEntry he =  Dns.GetHostEntry(Dns.GetHostName());
			foreach(IPAddress i in he.AddressList)
			{
				
				if(i.AddressFamily==AddressFamily.InterNetwork)
				{
					return i.ToString();
				}
			}
			return null;
		}
	}
}
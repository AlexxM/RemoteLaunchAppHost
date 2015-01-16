/*
 * Сделано в SharpDevelop.
 * Пользователь: 055makarov
 * Дата: 21.10.2014
 * Время: 14:01
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.ServiceModel;
namespace RemoteLaunchAppHost
{
 	[ServiceContract(CallbackContract = typeof(IHostCallback))]
    public interface IRemoteLaunchAppService
    {
        [OperationContract]
        void SubscribeRecieved();
        [OperationContract]
        //[FaultContract(typeof(Exception))]
        void LaunchProg(string command, string args, TimeSpan processLifeTime);
        [OperationContract]
        void SendMessage(string command);
        [OperationContract]
        void CloseProg();
        	
    }
}

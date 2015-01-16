/*
 * Сделано в SharpDevelop.
 * Пользователь: 055makarov
 * Дата: 20.10.2014
 * Время: 10:38
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.ServiceModel;
namespace RemoteLaunchAppHost
{
	/// <summary>
	/// Description of IHostCallback.
	/// </summary>
	public interface IHostCallback
	{
		[OperationContract(IsOneWay = true)]
		void RecievedData(string str);
	}
}

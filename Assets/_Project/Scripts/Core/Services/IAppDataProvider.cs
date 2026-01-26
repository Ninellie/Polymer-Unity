using System.Threading.Tasks;
using Core.Models;

public interface IAppDataProvider
{
    Task LoadAsync();
    ApplicationData AppData { get; } 
}
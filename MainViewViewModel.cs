using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using RevitAPITrainingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallUtils = Autodesk.Revit.DB.WallUtils;

namespace RevitAPITrainingUI
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public DelegateCommand SaveCommand { get; }
       public List<Element> PickedObjects { get; } = new List<Element>();  //пустой список
        public List<WallType> WallTypes { get; } = new List<WallType>();   //список элементов

        public WallType SelectedWallType { get; set; }   //выводит имя нашей системы

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            SaveCommand = new DelegateCommand(OnSaveCommand); //свойство которое сохраняет систему для выбранных труб
            PickedObjects = SelectionUtils.PickObjects(commandData); //выбор объектов(новый метод)
            WallTypes = WallUtils.GetWallType(commandData);   //Источник данных, библиотека, обращение к классу PipesUtils
        }

        private void OnSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (PickedObjects.Count == 0 || SelectedWallType == null)    //проверка системы, выбранных элементов
                return;

            using (var ts = new Transaction(doc, "Set system type")) //транзакция
            {
                ts.Start();

                foreach (var pickedObject in PickedObjects)
                {
                    if (pickedObject is Wall) //проверка
                    {
                        var oWall = pickedObject as Wall;
                        oWall.WallType=SelectedWallType; ///свойство установки системы
                        
                    }
                }

                ts.Commit();
            }

            RaiseCloseRequest();
        } 

        public event EventHandler CloseRequest; ///закрытие окна
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);

        }

    }

}

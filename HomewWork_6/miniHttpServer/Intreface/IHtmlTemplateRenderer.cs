namespace miniHttpServer.Intreface;

// Задание: Реализация шаблонизатора HTML на C#
// Цель: Создать простой шаблонизатор HTML, который может заменять переменные, выполнять условия $ifelse, и реализовывать циклы foreach на основе переданного объекта.
//     Условия:
// Создайте интерфейс IHtmlTemplateRenderer, содержащий три метода:
public interface IHtmlTemplateRenderer
{
    string RenderFromString(string htmlTemplate, object dataModel);
    string RenderFromFile(string filePath, object dataModel);
    string RenderToFile(string inputFilePath, string outputFilePath, object dataModel);
}
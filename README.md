# CSharpUTestDOM
CSharpUTestDOM é um projeto feito para automatizar a geração de testes unitários. A lógica se baseia nos dados de parâmetro e retorno em execução que são capturados por um observable.
Para executar na sua applicação deve seguir os seguintes passos:

> 1. Adicionar a referência de CSharpUTestDOM no seu projeto.

> 2. Adicionar a referência de PostSharp.dll (NuGet) https://www.postsharp.net/

> 3. Adicionar o Trace Listener no EntryPointMethod da sua aplicação

    using System.Diagnostics;
    using System;
    
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            App.sum(20, 50);
            Console.ReadKey();
        }
    }
    
> 4. Adicionar o atributo [Trace("Debug")] no método que vai ser gerado o teste.

> 5. Definir as configurações de saída no AppSetings do arquivo de configurações da sua aplicação.

      <appSettings>
        <add key="output-path" value="C:\Users\--user--\source\repos\UTest\Examples" />
        <add key="output-namespace" value="examples" />
        <add key="output-class" value="example" />
        <add key="output-mock" value="C:\Users\--user--\source\repos\UTest\Examples\objects" />
      </appSettings>
      
> 6. Executar o seu projeto, executar a rotina em que o método mapeado é chamado, remover o atributo do método e adicionar o arquivo .cs no seu projeto de testes.

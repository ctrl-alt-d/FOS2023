using Azure.AI.OpenAI;
using System.Text.Json;

namespace demo.Data;

public class JutgeService
{
    public class ValoracioRima
    {
        public float Puntuacio { get; set; }
        public string Valoracio { get; set; } = "";
    }

    private readonly OpenAIClient Client;


    public JutgeService()
    {
        // get OPENAI_API_KEY env variable
        var openAIKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        Client = new OpenAIClient(openAIKey);
    }

    public async Task ValoraRima(Rima rima)
    {        
        List<ChatMessage> messages = GetInitialChat();
        messages.Add(new ChatMessage(ChatRole.User, rima.Text));
        var chatCompletionsOptions = new ChatCompletionsOptions(messages);

        try
        {
            Azure.Response<ChatCompletions> response = await Client.GetChatCompletionsAsync(
                deploymentOrModelName: "gpt-3.5-turbo",
                chatCompletionsOptions);

            var json = response.Value.Choices[0].Message.Content;

            var valoracio =
                JsonSerializer.Deserialize<ValoracioRima>(json) ??
                throw new Exception("No s'ha pogut deserialitzar la resposta del chat");

            rima.Rate = valoracio.Puntuacio;
            rima.Valoracio = valoracio.Valoracio;
        }
        catch (Exception e)
        {
            rima.Rate = 0;
            rima.Valoracio = $"El chat no ho ha sabut valorar : {e.Message}";
        }
    }

    private static List<ChatMessage> GetInitialChat()
    {        
        return new()
        {
            //
            new ChatMessage(
                ChatRole.System,
                "Ets un jutge d'un concurs de rimes. Ets una màquina que només respon amb JSON. " +
                "La puntuació és del 0 al 10. Si la rima és molt dolenta es valora amb 0. " +
                "Si la rima és molt bona es valora amb 10. Si la rima és normal treurà un 5." +
                "Si la rima està relacionada amb l'open source té més puntuació."),

            //
            new ChatMessage(
                ChatRole.User,
                "Es JavaScript mola mogollón, lo usan en la China y lo usan en Japón"),
            new ChatMessage(
                ChatRole.Assistant,
                JsonSerializer.Serialize(
                    new ValoracioRima()
                    {
                        Puntuacio = 8.7f,
                        Valoracio = "Rima consonant. Relacionat amb el món de l'open source. No és inèdit"
                    }
                )),

            //
            new ChatMessage(
                ChatRole.User,
                "Em menjo una pizza i una hamburguesa"),
            new ChatMessage(
                ChatRole.Assistant,
                JsonSerializer.Serialize(
                    new ValoracioRima()
                    {
                        Puntuacio = 2.0f,
                        Valoracio = "Has participat, molt bé per participar-hi. No relacionat amb el món de l'open source. No rima"
                    }
                )),
            
            //
            new ChatMessage(
                ChatRole.User,
                "No te olvides de poner el Where en la sentència SQL Delete From"),
            new ChatMessage(
                ChatRole.Assistant,
                JsonSerializer.Serialize(
                    new ValoracioRima()
                    {
                        Puntuacio = 8.7f,
                        Valoracio = "Relacionat amb SQL, PostgreSQL és de codi obert. És graciós. La rima és molt fluixa"
                    }
                )),

            //
            new ChatMessage(
                ChatRole.User,
                "El meu gos es diu blu blup"),
            new ChatMessage(
                ChatRole.Assistant,
                JsonSerializer.Serialize(
                    new ValoracioRima()
                    {
                        Puntuacio = 1.1f,
                        Valoracio = "Gràcies per participar. No relacionat amb el món de l'open source. No rima. És off-topic"
                    }
                )),

        };
    }
}

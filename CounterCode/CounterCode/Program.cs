// See https://aka.ms/new-console-template for more information
using CounterCode;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.Reflection;

class Program
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public static FirestoreDb FirestoreDb { get; set; }

    public static DiscordClient DiscordClient { get; set; }

    private static ThreadLocal<Random> _random = new(() => new Random());
#pragma warning disable CS8603 // Possible null reference return.
    public static Random Random => _random.Value;
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    static async Task Main(string[] args)
    {
        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            Console.WriteLine(args);
        };

        var secrets = JsonConvert.DeserializeObject<Secrets>(File.ReadAllText("Secrets.json"));
        if(secrets == null)
        {
            return;
        }

        FirestoreDb = await FirestoreDb.CreateAsync(secrets.FirestoreProjectId);

        DiscordClient = new DiscordClient(new DiscordConfiguration()
        {
            Token = secrets.DiscordToken,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
        });

        //discord.MessageCreated += OnMessageCreated;

        SlashCommandsExtension slashCommands = DiscordClient.UseSlashCommands();

        Assembly assembly = Assembly.GetExecutingAssembly();
        foreach (Type type in assembly.GetTypes().Where(x => x.IsAssignableTo(typeof(ApplicationCommandModule))))
        {
            slashCommands.RegisterCommands(type, secrets.TestGuildId);
            slashCommands.RegisterCommands(type);
        }

        await DiscordClient.ConnectAsync();

        await Task.Delay(-1);
    }

    /*static async Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs eventArgs)
    {

    }*/
}
using System;
using System.Threading.Tasks;
using Octokit;
using Octokit.Internal;
using Octokit.Helpers;

namespace SteamInfoPlayerBot.Services
{
    public class GithubServices
    {
        internal GitHubClient _client = new GitHubClient(new ProductHeaderValue("steam-bot"));

        public GithubServices(string token)
        {
            _client.Credentials = new Credentials(token);
        }

        public Task<Issue> OpenIssue(System.Exception err, dynamic from)
        {
            NewIssue createIssue = new NewIssue(err.Message);
            createIssue.Body =
            $"## Source: {err.Source} \n" +
            $"**Telegram id:** {from.Id} \n" +
            $"**Full name:** {from.FirstName} {from.LastName}\n" +
            $"**Username:** {from.Username} \n\n" +
            $"```csharp\n" +
            $"{err.StackTrace}\n" +
            $"```";

            createIssue.Labels.Add("error");
            var issue = _client.Issue.Create("irsooti", "steam-bot", createIssue);
            return issue;
        }
    }
}
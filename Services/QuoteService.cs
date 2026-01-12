using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleCheck.Services
{
    public class QuoteService
    {
        private readonly HttpClient _httpClient;
        private readonly List<string> _localQuotes;

        public QuoteService()
        {
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            _httpClient.Timeout = TimeSpan.FromSeconds(5);

            // 内置本地名言库
            _localQuotes = new List<string>
            {
                "每一个不曾起舞的日子，都是对生命的辜负。",
                "生活不是等待风暴过去，而是学会在雨中翩翩起舞。",
                "今天的你比昨天更接近你的梦想。",
                "不要让昨天占用太多今天的时间。",
                "成功是日复一日那一点点小小努力的积累。",
                "做你热爱的事，成功自然会跟随你。",
                "每一次努力都不会被辜负，每一份坚持都是成功的积累。",
                "你今天的态度，决定了你明天的高度。",
                "保持一颗平常心，一切都会变得简单。",
                "专注当下，享受每一个瞬间。"
            };
        }

        public async Task<string> GetDailyQuoteAsync()
        {
            try
            {
                // 尝试从在线 API 获取
                var quote = await GetQuoteFromApiAsync();
                if (!string.IsNullOrEmpty(quote))
                {
                    return quote;
                }
            }
            catch (Exception ex)
            {
                // 网络请求失败，使用本地名言
                Console.WriteLine($"获取在线名言失败: {ex.Message}");
            }

            // 使用本地名言库
            return GetRandomLocalQuote();
        }
        
        // 保留同步方法，但优先使用本地名言
        public string GetDailyQuote()
        {
            return GetRandomLocalQuote();
        }

        private async Task<string> GetQuoteFromApiAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://v1.hitokoto.cn/?c=a&c=b&c=c&c=d&c=e&c=f&c=g");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<HitokotoResult>(content);
                    return result?.Hitokoto ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Hitokoto API 请求失败: {ex.Message}");
            }

            return string.Empty;
        }

        private string GetRandomLocalQuote()
        {
            var random = new Random();
            int index = random.Next(0, _localQuotes.Count);
            return _localQuotes[index];
        }

        private class HitokotoResult
        {
            public required string Hitokoto { get; set; }
        }
    }
}
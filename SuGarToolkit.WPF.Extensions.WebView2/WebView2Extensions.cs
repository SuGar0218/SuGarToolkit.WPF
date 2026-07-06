
using Microsoft.Web.WebView2.Wpf;

namespace SuGarToolkit.WPF.Extensions.WebView2
{
    public static class WebView2Extensions
    {
        public static Task SelectAll(this IWebView2 webView2) => webView2.ExecuteScriptAsync(js_SelectAll);

        public static Task ClearSelection(this IWebView2 webView2) => webView2.ExecuteScriptAsync(js_ClearSelection);

        private const string js_SelectAll = @"
            try {
                // 优先使用 execCommand
                document.execCommand('selectAll');
            } catch(e) {
                // 降级方案：手动选中 body 内容
                const range = document.createRange();
                range.selectNodeContents(document.body);
                const sel = window.getSelection();
                sel.removeAllRanges();
                sel.addRange(range);
            }
        ";

        private const string js_ClearSelection = @"
            window.getSelection().removeAllRanges();
        ";
    }
}

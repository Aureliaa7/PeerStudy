using Blazorise.RichTextEdit;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace PeerStudy.Shared.TextEditorComponent
{
    public partial class PeerStudyTextEditor
    {
        [Parameter]
        public bool IsReadOnly { get; set; }

        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public string HtmlContent { get; set; }

        [Parameter]
        public EventCallback<string> OnSave { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }


        private RichTextEdit editorRef { get; set; }

        private const string buttonStyles = "display: inline-block;";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await editorRef.SetHtmlAsync(HtmlContent);
        }

        private async Task Save()
        {
            await OnSave.InvokeAsync(await editorRef.GetHtmlAsync());
        }

        private async Task Cancel()
        {
            await OnCancel.InvokeAsync();
        }
    }
}

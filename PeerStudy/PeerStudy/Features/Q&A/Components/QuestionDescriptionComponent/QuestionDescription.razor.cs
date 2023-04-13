using Blazorise.RichTextEdit;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace PeerStudy.Features.Q_A.Components.QuestionDescriptionComponent
{
    public partial class QuestionDescription
    {
        [Parameter]
        public string HtmlContent { get; set; }

        private RichTextEdit editorRef;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await editorRef.SetHtmlAsync(HtmlContent);
        }
    }
}

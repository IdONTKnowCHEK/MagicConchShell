using MagicConchShell.Enum;

namespace MagicConchShell.Dtos.Message
{
    public class TextMessageDto : BaseMessageDto
    {
        public TextMessageDto()
        {
            Type = MessageTypeEnum.Text;
        }
        public string Text { get; set; }
    }
}

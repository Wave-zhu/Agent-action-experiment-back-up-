using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenAI
{
    public abstract class GPTBase : MonoBehaviour
    {
        protected OpenAIApi openai
                = new OpenAIApi("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", "org-2AMMVntRilvMEGHOMGOQwGRz");
        protected List<ChatMessage> messages = new List<ChatMessage>();
    }
}

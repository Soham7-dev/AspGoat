(() => {
  // --- configure endpoint ---
  const ENDPOINT = '/LLM/InsecureOutputHandling';
  const NOTES = ''; // optional: passed as ?notes=... if you want

  // --- DOM ---
  const chatEl   = document.getElementById('chat');
  const promptEl = document.getElementById('prompt');
  const sendBtn  = document.getElementById('send');

  // --- helpers ---
  const escapeHtml = (s) =>
    s.replace(/[&<>"']/g, c => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));

  function addMessage(role, text) {
    const wrap = document.createElement('div');
    wrap.className = `msg ${role}`;
    const bubble = document.createElement('div');
    bubble.className = 'bubble';

    // Vulnerable as raw HTML output from the AI response is being reflected
    bubble.innerHTML = text;
    wrap.appendChild(bubble);
    chatEl.appendChild(wrap);
    chatEl.scrollTop = chatEl.scrollHeight;
  }

  function showTyping() {
    const el = document.createElement('div');
    el.className = 'typing';
    el.textContent = 'Assistant is typing…';
    el.dataset.typing = '1';
    chatEl.appendChild(el);
    chatEl.scrollTop = chatEl.scrollHeight;
    return el;
  }
  function hideTyping(el) { if (el && el.parentNode) el.parentNode.removeChild(el); }

  // autosize textarea
  promptEl.addEventListener('input', () => {
    promptEl.style.height = 'auto';
    const max = 8 * 24; // ~8 lines
    promptEl.style.height = Math.min(promptEl.scrollHeight, max) + 'px';
  });

  function buildUrl() {
    const qs = new URLSearchParams();
    if (NOTES) qs.set('notes', NOTES);
    const q = qs.toString();
    return q ? `${ENDPOINT}?${q}` : ENDPOINT;
  }

  async function send() {
    const text = promptEl.value.trim();
    if (!text) return;

    addMessage('user', text);
    promptEl.value = '';
    promptEl.style.height = 'auto';
    sendBtn.disabled = true;

    const typingEl = showTyping();
    try {
      const res = await fetch(buildUrl(), {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json; charset=UTF-8',
          'Accept': 'application/json'
        },
        body: JSON.stringify({ prompt: text })
      });

      const data = await res.json();
      hideTyping(typingEl);

      if (!res.ok) {
        addMessage('assistant', (data && data.error) ? data.error : `Error ${res.status}`);
      } else {
        addMessage('assistant', (data && data.reply) ? data.reply : '');
                
        // Vulnerable as the response from the AI Chatbot is executed via the eval function
        try
        {
            eval(data.reply);
        }
        catch(e)
        {
            console.log(e);
        }

      }
    } catch (e) {
      hideTyping(typingEl);
      addMessage('assistant', 'Network error.');
    } finally {
      sendBtn.disabled = false;
      promptEl.focus();
    }
  }

  sendBtn.addEventListener('click', send);
  // Enter to send, Shift+Enter for newline
  promptEl.addEventListener('keydown', (e) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      send();
    }
  });

  promptEl.focus();
})();
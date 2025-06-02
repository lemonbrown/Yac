<script>
  let query = 'player: Mahomes stats';
  let result = null;
  let error = null;

  async function runQuery() {
    error = null;
    try {
      const res = await fetch('http://localhost:5000/query', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ query })
      });
      if (!res.ok) throw new Error('Bad query');
      result = await res.json();
    } catch (e) {
      error = e.message;
    }
  }
</script>

<div style="padding-bottom:3px">
  <button on:click={runQuery}>
  ▶
  </button>
  shift + enter
</div>
<textarea bind:value={query} 
cols="48" rows="6" style="resize:none"/>


{#if error}
  <p class="text-red-500 mt-2">{error}</p>
{/if}

{#if result}
  <table class="mt-4 border">
    <thead>
      <tr>
        {#each Object.keys(result[0]) as col}
          <th class="p-2 border">{col}</th>
        {/each}
      </tr>
    </thead>
    <tbody>
      {#each result as row}
        <tr>
          {#each Object.values(row) as val}
            <td class="p-2 border">{val}</td>
          {/each}
        </tr>
      {/each}
    </tbody>
  </table>
{/if}

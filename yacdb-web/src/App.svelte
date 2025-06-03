<script>
  let query = 'player: Mahomes stats';
  let result = null;
  let error = null;

  async function runQuery() {
    error = null;
    try {
      const res = await fetch('http://localhost:5000/yql', {
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
<div class="" style="padding-left:8px; margin-bottom:12px; font-size:32px; font-weight: bold">
yac-db
</div>
<div class="container">

  <div style="flex: 1; display: flex; flex-direction: column;padding-right:12px; max-width:1200px">
    <div style="padding-bottom: 6px; display: flex; align-items: center; gap: 8px;">
      <button on:click={runQuery} style="flex-shrink: 0;">
        ▶
      </button>
      <span style="font-size: 12px; color: #555;">shift + enter</span>
    </div>

    <textarea
      bind:value={query}
      rows="6"
      style="width: 100%; resize: none; font-family: monospace; font-size: 14px;"
    ></textarea>
  

{#if error}
  <p class="text-red-500 mt-2">{error}</p>
{/if}

{#if result}
  <table class="mt-4 border results-table">
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
  </div>
  
  <!-- <div style="min-width: 320px; border: 1px solid transparent;">
  </div> -->

</div>

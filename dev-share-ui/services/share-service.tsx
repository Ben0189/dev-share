export async function submitSharedResource(
  url: string,
  comment: string
): Promise<string> {
  const controller = new AbortController();
  const timeoutId = setTimeout(() => controller.abort(), 6000);

  try {
    const res = await fetch(
      `${process.env.NEXT_PUBLIC_API_BASE_URL_WITH_API}/share`,
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ url: url, insight: comment }),
        signal: controller.signal,
      }
    );

    clearTimeout(timeoutId);

    if (!res.ok) {
      throw new Error('Share request failed');
    }

    const data = await res.json();
    return data.taskId;
  } catch (err) {
    clearTimeout(timeoutId);
    throw err;
  }
}

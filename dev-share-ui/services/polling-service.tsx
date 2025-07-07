export async function pollShareStatus(
    taskId: string,
    options: { maxPolls?: number; interval?: number } = {}
  ): Promise<"success" | "failed" | "timeout"> {
    const maxPolls = options.maxPolls ?? 5;
    const interval = options.interval ?? 3000;
    let pollCount = 0;
  
    return new Promise((resolve) => {
      const poll = async () => {
        pollCount++;
  
        try {
          const res = await fetch(`${process.env.NEXT_PUBLIC_API_BASE_URL_WITH_API}/share/status/${taskId}`);
          const data = await res.json();
  
          if (data.status === "success") return resolve("success");
          if (data.status === "failed") return resolve("failed");
  
          if (pollCount >= maxPolls) return resolve("timeout");
  
          setTimeout(poll, interval);
        } catch (e) {
          return resolve("timeout"); // Treat error as timeout for now
        }
      };
  
      poll();
    });
  }

//query share-status without polling
  export async function shareStatus(
    taskId: string,
    options: { maxPolls?: number; interval?: number } = {}
  ): Promise<"success" | "failed" | "timeout"> {
    
    return new Promise((resolve) => {
      const query = async () => {
        
        try {
          const res = await fetch(`${process.env.NEXT_PUBLIC_API_BASE_URL_WITH_API}/share/status/${taskId}`);
          const data = await res.json();
  
          if (data.status === "success") return resolve("success");
          if (data.status === "failed") return resolve("failed");
  
        } catch (e) {
          return resolve("timeout"); // Treat error as timeout for now
        }
    }});
  }
"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { Link as LinkIcon } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { toast } from "sonner";
import Navbar from "@/components/Navbar";

export default function ShareResourcePage() {
  const [url, setUrl] = useState("");
  const [prompt, setPrompt] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [urlError, setUrlError] = useState("");
  const router = useRouter();

  const validateUrl = (url: string) => {
    try {
      new URL(url);
      setUrlError("");
      return true;
    } catch (e) {
      setUrlError("Please enter a valid URL (including http:// or https://)");
      return false;
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateUrl(url)) {
      return;
    }
    
    setIsSubmitting(true);
    
    // TODO: Integration point for AI processing
    // 1. Send URL and prompt to backend
    // 2. Backend will:
    //    - Scrape the URL for content
    //    - Use AI to generate title, description, and tags
    //    - Store the processed resource
    // Example:
    // const resource = await processResourceWithAI({ url, prompt });
    
    // Simulate API call
    
    const res = await fetch('https://localhost:7122/api/share',{
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        url: url,
        prompt: prompt
      }),
    })
    if(!res.ok){
        toast.error("Uh oh! Something went wrong. Please try again later.");
        throw new Error('Request failed');
    }

    toast.success("Resource shared!", {duration: 2500});
    setIsSubmitting(false);
    router.push("/");
  };

  return (
    <main className="min-h-screen bg-background">
        <Navbar />
      <div className="container px-4 py-8 mx-auto max-w-2xl">
        <Card className="animate-in fade-in-50 slide-in-from-bottom-8 duration-300">
          <CardHeader>
            <CardTitle className="text-2xl">Share a Resource</CardTitle>
            <CardDescription>
              Share a valuable developer resource. Our AI will analyze it and add relevant details.
            </CardDescription>
          </CardHeader>
          <form onSubmit={handleSubmit}>
            <CardContent className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="url">Resource URL <span className="text-destructive">*</span></Label>
                <div className="relative">
                  <LinkIcon className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                  <Input
                    id="url"
                    type="text"
                    placeholder="https://example.com/resource"
                    value={url}
                    onChange={(e) => setUrl(e.target.value)}
                    className="pl-10"
                    required
                  />
                </div>
                {urlError && <p className="text-sm text-destructive">{urlError}</p>}
              </div>
              
              <div className="space-y-2">
                <Label htmlFor="prompt">Additional Context (Optional)</Label>
                <Textarea
                  id="prompt"
                  placeholder="Add any details that might help our AI better understand this resource (e.g., 'Great for React beginners')"
                  value={prompt}
                  onChange={(e) => setPrompt(e.target.value)}
                  rows={3}
                />
              </div>
            </CardContent>
            <CardFooter className="flex justify-between">
              <Button 
                type="button" 
                variant="outline" 
                onClick={() => router.push("/")}
              >
                Cancel
              </Button>
              <Button 
                type="submit" 
                disabled={isSubmitting || !url.trim()}
              >
                {isSubmitting ? (
                  <>
                    <div className="h-4 w-4 mr-2 rounded-full border-2 border-current border-t-transparent animate-spin" />
                    Submitting...
                  </>
                ) : (
                  "Share Resource"
                )}
              </Button>
            </CardFooter>
          </form>
        </Card>
      </div>
    </main>
  );
}
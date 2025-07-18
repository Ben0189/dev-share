'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { Link as LinkIcon, MessageSquare } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';
import { toast } from 'sonner';
import { submitSharedResource } from '@/services/share-service';
import { pollShareStatus } from '@/services/polling-service';

export default function ShareResourcePage() {
  const [url, setUrl] = useState('');
  const [comment, setComment] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [urlError, setUrlError] = useState('');
  const router = useRouter();

  const validateUrl = (url: string) => {
    try {
      new URL(url);
      setUrlError('');
      return true;
    } catch (e) {
      setUrlError('Please enter a valid URL (including http:// or https://)');
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
    //set the max processing time
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 6000);

    //TODO: Move this to "share" service
    try {
      const taskId = await submitSharedResource(url, comment);
      toast.success('Processing resource...please check status later', {
        duration: 2500,
      });

      const result = await pollShareStatus(taskId, {
        maxPolls: 5,
        interval: 3000,
      });

      if (result === 'success') {
        toast.success('Resource processing completed!');
      } else if (result === 'failed') {
        toast.error('Resource processing failed.');
      } else {
        toast.error('Resource processing timed out.');
      }

      router.push('/');
    } catch (err: any) {
      if (err.name === 'AbortError') {
        toast.error('Request timed out after 5 seconds.');
      } else {
        toast.error('An unexpected error occurred.');
      }
    }
    setIsSubmitting(false);
    router.push('/');
  };

  return (
    <main className="min-h-screen bg-background">
      <div className="container px-4 py-8 mx-auto max-w-2xl">
        <Card className="animate-in fade-in-50 slide-in-from-bottom-8 duration-300 rounded-2xl shadow-lg bg-white">
          <CardHeader>
            <CardTitle className="text-2xl">Share a Resource</CardTitle>
            <CardDescription>
              Share a valuable developer resource. Our AI will analyze it and
              add relevant details.
            </CardDescription>
            <hr className="my-4 border-muted" />
          </CardHeader>
          <form onSubmit={handleSubmit}>
            <CardContent className="space-y-6">
              <div className="space-y-2">
                <Label htmlFor="url">
                  Resource URL <span className="text-destructive">*</span>
                </Label>
                <div className="relative">
                  <LinkIcon className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                  <Input
                    id="url"
                    type="text"
                    placeholder="https://example.com/resource"
                    value={url}
                    onChange={(e) => setUrl(e.target.value)}
                    className="pl-10 focus:ring-2 focus:ring-primary/30 hover:border-primary transition-all duration-200"
                    required
                  />
                </div>
                {urlError && (
                  <p className="text-sm text-destructive">{urlError}</p>
                )}
              </div>
              <div className="space-y-2">
                <Label htmlFor="comment" className="flex items-center gap-2">
                  <MessageSquare className="h-4 w-4 text-muted-foreground" />
                  Your Comment
                </Label>
                <Textarea
                  id="comment"
                  placeholder="Share your thoughts, experience, or recommendation about this resource (e.g. I read this docs and I learn a lot... very recommend for beginner to React!)"
                  value={comment}
                  onChange={(e) => setComment(e.target.value)}
                  rows={3}
                  required
                  className="focus:ring-2 focus:ring-primary/30 hover:border-primary transition-all duration-200"
                />
                <div className="flex items-center justify-between text-xs text-muted-foreground mt-1">
                  <span>Let others know why you recommend this resource.</span>
                  <span>{comment.length}/200</span>
                </div>
              </div>
            </CardContent>
            <CardFooter className="flex flex-col sm:flex-row sm:justify-between gap-2 mt-2">
              <Button
                type="button"
                variant="outline"
                onClick={() => router.push('/')}
                className="w-full sm:w-auto"
              >
                Cancel
              </Button>
              <Button
                type="submit"
                disabled={isSubmitting || !url.trim()}
                className="w-full sm:w-auto bg-primary text-white font-semibold px-6 py-2 rounded-lg flex items-center justify-center gap-2 shadow-md hover:bg-primary/90 transition-all duration-200"
              >
                {isSubmitting ? (
                  <>
                    <div className="h-4 w-4 mr-2 rounded-full border-2 border-current border-t-transparent animate-spin" />
                    Submitting...
                  </>
                ) : (
                  <>Share Resource</>
                )}
              </Button>
            </CardFooter>
          </form>
        </Card>
      </div>
    </main>
  );
}

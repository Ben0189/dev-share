"use client";

import { useState } from "react";
import { Plus, Link as LinkIcon, X } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Badge } from "@/components/ui/badge";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";

interface ShareResourceFormProps {
  onClose: () => void;
  onSubmit: (resource: any) => void;
}

export default function ShareResourceForm({ onClose, onSubmit }: ShareResourceFormProps) {
  const [url, setUrl] = useState("");
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [comment, setComment] = useState("");
  const [tagInput, setTagInput] = useState("");
  const [tags, setTags] = useState<string[]>([]);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [urlError, setUrlError] = useState("");

  const handleAddTag = () => {
    if (tagInput.trim() && !tags.includes(tagInput.trim())) {
      setTags([...tags, tagInput.trim()]);
      setTagInput("");
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "Enter") {
      e.preventDefault();
      handleAddTag();
    }
  };

  const removeTag = (tagToRemove: string) => {
    setTags(tags.filter(tag => tag !== tagToRemove));
  };

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
    
    if (!title.trim()) {
      return;
    }
    
    setIsSubmitting(true);
    
    // Create new resource object
    const newResource = {
      url,
      title,
      description,
      tags,
      date: new Date().toISOString(),
      comment,
    };
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    onSubmit(newResource);
    
    // Reset form
    setUrl("");
    setTitle("");
    setDescription("");
    setTags([]);
    setComment("");
    setIsSubmitting(false);
    onClose();
  };

  return (
    <Card className="w-full max-w-2xl mx-auto animate-in fade-in-50 slide-in-from-bottom-8 duration-300">
      <CardHeader>
        <CardTitle className="text-2xl">Share a Resource</CardTitle>
        <CardDescription>
          Help the developer community by sharing valuable resources you've found
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
            <Label htmlFor="title">Title <span className="text-destructive">*</span></Label>
            <Input
              id="title"
              type="text"
              placeholder="Resource title"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              required
            />
          </div>
          
          <div className="space-y-2">
            <Label htmlFor="comment">Your Comment <span className="text-destructive">*</span></Label>
            <Textarea
              id="comment"
              placeholder="Share your thoughts, experience, or recommendation about this resource (e.g. I read this docs and I learn a lot... very recommend for beginner to React!)"
              value={comment}
              onChange={(e) => setComment(e.target.value)}
              rows={3}
              required
            />
          </div>
          
          <div className="space-y-2">
            <Label htmlFor="tags">Tags</Label>
            <div className="flex items-center space-x-2">
              <Input
                id="tags"
                type="text"
                placeholder="Add tags (e.g., React, TypeScript)"
                value={tagInput}
                onChange={(e) => setTagInput(e.target.value)}
                onKeyDown={handleKeyDown}
              />
              <Button 
                type="button" 
                size="icon" 
                variant="outline" 
                onClick={handleAddTag}
              >
                <Plus className="h-4 w-4" />
              </Button>
            </div>
            <div className="flex flex-wrap gap-2 mt-2">
              {tags.map((tag) => (
                <Badge key={tag} variant="secondary" className="pl-2 pr-1 py-1">
                  {tag}
                  <Button 
                    variant="ghost" 
                    size="icon" 
                    className="h-4 w-4 ml-1 hover:bg-destructive/20" 
                    onClick={() => removeTag(tag)}
                  >
                    <X className="h-3 w-3" />
                  </Button>
                </Badge>
              ))}
            </div>
          </div>
        </CardContent>
        <CardFooter className="flex justify-between">
          <Button 
            type="button" 
            variant="outline" 
            onClick={onClose}
          >
            Cancel
          </Button>
          <Button 
            type="submit" 
            disabled={isSubmitting || !url.trim() || !title.trim()}
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
  );
}
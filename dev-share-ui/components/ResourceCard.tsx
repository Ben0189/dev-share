"use client";

import { useState } from "react";
import { ExternalLink, ThumbsUp, Bookmark, Link as LinkIcon, MessageSquare } from "lucide-react";
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Resource } from "@/lib/types";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";

interface ResourceCardProps {
  resource: Resource;
  onAction: (id: string, action: 'like' | 'bookmark') => void;
}

function timeAgo(dateString: string) {
  const now = new Date();
  const date = new Date(dateString);
  const diff = now.getTime() - date.getTime();
  const years = Math.floor(diff / (1000 * 60 * 60 * 24 * 365));
  if (years > 0) return `over ${years} year${years > 1 ? 's' : ''} ago`;
  const months = Math.floor(diff / (1000 * 60 * 60 * 24 * 30));
  if (months > 0) return `${months} month${months > 1 ? 's' : ''} ago`;
  const days = Math.floor(diff / (1000 * 60 * 60 * 24));
  if (days > 0) return `${days} day${days > 1 ? 's' : ''} ago`;
  return 'recently';
}

export default function ResourceCard({ resource, onAction }: ResourceCardProps) {
  return (
    <Card className="flex flex-col justify-between min-h-[320px] min-w-0 p-0 rounded-xl border shadow-sm bg-white transition-all duration-300 hover:shadow-md">
      {/* Author and meta */}
      <div className="flex items-center gap-3 px-6 pt-6 pb-2">
        <img
          src={resource.authorAvatar}
          alt={resource.authorName}
          className="w-10 h-10 rounded-full object-cover border"
        />
        <div className="flex flex-col">
          <span className="font-medium text-sm text-foreground leading-tight">{resource.authorName}</span>
          <span className="text-xs text-muted-foreground">{timeAgo(resource.createdAt)}</span>
        </div>
      </div>
      {/* User comment (collapsed, tooltip on hover) */}
      {resource.comment && (
        <TooltipProvider>
          <Tooltip>
            <TooltipTrigger asChild>
              <div className="px-6 pb-1 flex items-start gap-2 cursor-pointer">
                <MessageSquare className="h-4 w-4 text-muted-foreground mt-0.5" />
                <span className="italic text-muted-foreground text-sm line-clamp-1 max-w-full" style={{overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap'}}>{resource.comment}</span>
              </div>
            </TooltipTrigger>
            <TooltipContent side="top" className="max-w-xs whitespace-pre-line">
              {resource.comment}
            </TooltipContent>
          </Tooltip>
        </TooltipProvider>
      )}
      {/* Title and description */}
      <div className="flex-1 px-6 pt-2 pb-0 flex flex-col">
        <h3 className="text-lg font-semibold leading-snug mb-1 text-foreground">{resource.title}</h3>
        <p className="text-sm text-muted-foreground mb-3 line-clamp-2">{resource.description}</p>
        <div className="flex flex-wrap gap-2 mb-2 mt-auto">
          {resource.tags.map((tag) => (
            <Badge key={tag} variant="outline" className="text-xs font-medium">#{tag}</Badge>
          ))}
        </div>
      </div>
      {/* Footer actions */}
      <div className="flex items-center justify-between px-6 py-4 border-t mt-2">
        <div className="flex items-center gap-6 text-muted-foreground">
          <button
            className={`flex items-center gap-1 text-xs hover:text-primary transition-colors ${resource.isLiked ? 'font-semibold text-primary' : ''}`}
            onClick={() => onAction(resource.id, 'like')}
            aria-label="Like"
          >
            <ThumbsUp className="h-4 w-4" />
            {resource.likes}
          </button>
          <span className="flex items-center gap-1 text-xs">
            <LinkIcon className="h-4 w-4" />
            {resource.linkClicks}
            <span className="ml-1">Link</span>
          </span>
        </div>
        <Button
          asChild
          size="sm"
          className="px-5 font-semibold text-base bg-primary hover:bg-primary/90 text-white"
        >
          <a href={resource.url} target="_blank" rel="noopener noreferrer" className="flex items-center gap-2">
            Visit
            <ExternalLink className="h-4 w-4" />
          </a>
        </Button>
      </div>
    </Card>
  );
}
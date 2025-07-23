'use client';

import {
  ExternalLink,
  ThumbsUp,
  Bookmark,
  Link as LinkIcon,
  MessageSquare,
} from 'lucide-react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Resource } from '@/lib/types';
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from '@/components/ui/tooltip';

interface ResourceCardProps {
  resource: Resource;
  onAction: (id: string, action: 'like' | 'bookmark') => void;
  isAIGenerated?: boolean;
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

export default function ResourceCard({
  resource,
  onAction,
  isAIGenerated = false,
}: ResourceCardProps) {
  return (
    <Card
      className={`flex flex-col justify-between min-h-[320px] min-w-0 p-0 rounded-xl border shadow-sm bg-white transition-all duration-300 hover:shadow-md relative ${
        isAIGenerated ? 'border-indigo-500 ring-2 ring-indigo-200' : ''
      }`}
    >
      {/* AI Generated badge */}
      {isAIGenerated && (
        <span className="absolute top-4 left-4 z-10 bg-indigo-100 text-indigo-700 px-3 py-1 rounded-full text-xs font-semibold shadow border border-indigo-200 flex items-center gap-1">
          <svg
            className="w-4 h-4 mr-1"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            viewBox="0 0 20 20"
          >
            <path d="M10 2a1 1 0 0 1 1 1v1.07a7.002 7.002 0 0 1 5.93 5.93H18a1 1 0 1 1 0 2h-1.07a7.002 7.002 0 0 1-5.93 5.93V18a1 1 0 1 1-2 0v-1.07a7.002 7.002 0 0 1-5.93-5.93H2a1 1 0 1 1 0-2h1.07a7.002 7.002 0 0 1 5.93-5.93V3a1 1 0 0 1 1-1z" />
          </svg>
          AI Generated
        </span>
      )}
      {/* Bookmark button top right */}
      <button
        className={`absolute top-4 right-4 z-10 bg-white/80 rounded-full p-2 shadow-sm border border-muted-foreground/10 hover:bg-primary/10 transition-colors ${
          resource.isBookmarked ? 'text-primary' : 'text-muted-foreground'
        }`}
        onClick={() => onAction(resource.id, 'bookmark')}
        aria-label={resource.isBookmarked ? 'Remove bookmark' : 'Bookmark'}
      >
        <Bookmark
          className={`h-5 w-5 ${resource.isBookmarked ? 'fill-current' : ''}`}
        />
      </button>
      {/* Author and meta */}
      <div className="flex items-center gap-3 px-6 pt-6 pb-2">
        <img
          src={resource.authorAvatar}
          alt={resource.authorName}
          className="w-8 h-8 rounded-full object-cover border fill-gray-600"
        />
        <div className="flex flex-col">
          <span className="font-medium text-sm text-foreground leading-tight">
            {resource.authorName}
          </span>
          <span className="text-xs text-muted-foreground">
            {timeAgo(resource.createdAt)}
          </span>
        </div>
      </div>
      {/* User comment (collapsed, tooltip on hover) */}
      {resource.comment && (
        <TooltipProvider>
          <Tooltip>
            <TooltipTrigger asChild>
              <div className="px-6 pb-1 flex items-start gap-2 cursor-pointer">
                <MessageSquare className="h-4 w-4 text-muted-foreground mt-0.5" />
                <span
                  className="italic text-muted-foreground text-sm line-clamp-1 max-w-full"
                  style={{
                    overflow: 'hidden',
                    textOverflow: 'ellipsis',
                    whiteSpace: 'nowrap',
                  }}
                >
                  {resource.comment}
                </span>
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
        <h3 className="text-lg font-semibold leading-snug mb-1 text-foreground">
          {resource.title}
        </h3>
        <p className="text-sm text-muted-foreground mb-3 line-clamp-2">
          {resource.description}
        </p>
        <div className="flex flex-wrap gap-2 mb-2 mt-auto">
          {resource.tags.map((tag) => (
            <Badge key={tag} variant="outline" className="text-xs font-medium">
              #{tag}
            </Badge>
          ))}
        </div>
      </div>
      {/* Footer actions */}
      <div className="flex items-center justify-between px-6 py-4 border-t mt-2">
        <div className="flex items-center gap-6 text-muted-foreground">
          <button
            className={`flex items-center gap-1 text-xs hover:text-primary transition-colors ${
              resource.isLiked ? 'font-semibold text-primary' : ''
            }`}
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
          <a
            href={resource.url}
            target="_blank"
            rel="noopener noreferrer"
            className="flex items-center gap-2"
          >
            Visit
            <ExternalLink className="h-4 w-4" />
          </a>
        </Button>
      </div>
    </Card>
  );
}

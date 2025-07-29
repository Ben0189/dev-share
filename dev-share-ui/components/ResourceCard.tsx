'use client';

import {
  ExternalLink,
  ThumbsUp,
  Bookmark,
  Link as LinkIcon,
} from 'lucide-react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Resource } from '@/lib/types';
import ReadMoreArea from '@foxeian/react-read-more';

interface ResourceCardProps {
  resource: Resource;
  onAction: (id: string, action: 'like' | 'bookmark') => void;
}

export default function ResourceCard({
  resource,
  onAction,
}: ResourceCardProps) {
  return (
    <Card
      className={`flex flex-col justify-between min-h-[280px] px-2 pt-4 rounded-xl border-2 shadow-sm bg-background/80 transition-all duration-300 hover:shadow-md relative'
      }`}
    >
      {/* Title and description */}
      <div className="flex-1 px-6 flex flex-col">
        <div className="flex flex-row justify-between items-center py-2">
          <h1 className="text-xl font-semibold leading-snug mb-1 text-foreground">
            {resource.title}
          </h1>
          <button
            className={`bg-background/80 rounded-full p-2 shadow-sm border-2 border-muted-foreground/10 hover:bg-primary/10 transition-colors ${
              resource.isBookmarked ? 'text-primary' : 'text-muted-foreground'
            }`}
            onClick={() => onAction(resource.id, 'bookmark')}
            aria-label={resource.isBookmarked ? 'Remove bookmark' : 'Bookmark'}
          >
            <Bookmark
              className={`h-5 w-5 ${
                resource.isBookmarked ? 'fill-current' : ''
              }`}
            />
          </button>
        </div>

        <ReadMoreArea
          className="flex flex-col justify-start" // classes styles of main div (tailwind)
          expandLabel="Read more" // Expand Label
          collapseLabel="Read less" // Collapse Label
          textClassName="text-sm text-muted-foreground" // classes styles of text (tailwind)
          buttonClassName="text-foreground no-underline text-sm" // classes styles of button (tailwind)
          lettersLimit={80} // limit of letters (100 letters)
        >
          {resource.description}
        </ReadMoreArea>
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
          className="p px-4 bg-foreground/90 hover:bg-primary/90 text-background"
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

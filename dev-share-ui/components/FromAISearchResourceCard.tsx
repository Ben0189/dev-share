import { ExternalLink, ThumbsUp, Link as LinkIcon, MessageSquare, Sparkles } from "lucide-react";
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Resource } from "@/lib/types";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";

interface FromAISearchResourceCardProps {
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

export default function FromAISearchResourceCard({ resource, onAction }: FromAISearchResourceCardProps) {
  return (
    <Card className="relative flex flex-col justify-between min-h-[340px] min-w-0 p-0 rounded-2xl border-2 border-transparent bg-gradient-to-br from-indigo-50 via-white to-purple-100 shadow-xl overflow-hidden">
      {/* AI Gradient border */}
      <div className="absolute top-0 left-0 w-full h-2 bg-gradient-to-r from-indigo-400 via-fuchsia-400 to-purple-500" />
      {/* Card content */}
      <div className="flex-1 px-6 pt-8 pb-0 flex flex-col">
        <h3 className="text-lg font-bold leading-snug text-indigo-800 flex items-center gap-2 m-0 p-0">
          {resource.title.replace(/\s*\(AI Search\)/, "")}
        </h3>
        <p className="text-sm text-indigo-600 mb-3 line-clamp-2 font-medium">{resource.description}</p>
        {resource.comment && (
          <TooltipProvider>
            <Tooltip>
              <TooltipTrigger asChild>
                <div className="pb-1 flex items-start gap-2 cursor-pointer">
                  <MessageSquare className="h-4 w-4 text-fuchsia-400 mt-0.5" />
                  <span className="italic text-fuchsia-700 text-sm line-clamp-1 max-w-full" style={{overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap'}}>{resource.comment}</span>
                </div>
              </TooltipTrigger>
              <TooltipContent side="top" className="max-w-xs whitespace-pre-line">
                {resource.comment}
              </TooltipContent>
            </Tooltip>
          </TooltipProvider>
        )}
        <div className="flex flex-wrap gap-2 mb-2 mt-auto">
          {resource.tags.map((tag) => (
            <span key={tag} className="bg-fuchsia-100 text-fuchsia-700 px-2 py-0.5 rounded-full text-xs font-semibold">#{tag}</span>
          ))}
        </div>
      </div>
      {/* Footer actions */}
      <div className="flex items-center justify-between px-6 py-4 border-t-2 border-indigo-100 mt-2 bg-gradient-to-r from-indigo-50 to-purple-50">
        <div className="flex items-center gap-6 text-fuchsia-700">
          <button
            className={`flex items-center gap-1 text-xs hover:text-fuchsia-500 transition-colors ${resource.isLiked ? 'font-semibold text-fuchsia-600' : ''}`}
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
          className="px-5 font-semibold text-base bg-gradient-to-r from-indigo-500 to-fuchsia-500 hover:from-fuchsia-500 hover:to-indigo-500 text-white shadow"
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
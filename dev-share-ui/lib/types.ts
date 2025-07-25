export interface Resource {
  id: string;
  title: string;
  description: string;
  url: string;
  imageUrl: string;
  tags: string[];
  likes: number;
  date: string;
  isLiked: boolean;
  isBookmarked: boolean;
  recommended: boolean;
  authorName: string;
  authorAvatar: string;
  linkClicks: number;
  createdAt: string;
  comment: string;
  isAIGenerated?: boolean;
}

export interface VectorSearchResultDTO {
  url: string;
  content: string;
  title: string;
}
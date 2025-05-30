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
}
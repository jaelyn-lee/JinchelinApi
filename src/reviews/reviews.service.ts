import { Injectable, NotFoundException } from '@nestjs/common';
import { PrismaService } from '../prisma/prisma.service';
import { Prisma } from '@prisma/client';

type ReviewWithRelations = Prisma.ReviewGetPayload<{
  include: { dish: { include: { category: true } } };
}>;
@Injectable()
export class ReviewsService {
  constructor(private prisma: PrismaService) {}

  async findAll() {
    const reviews = await this.prisma.review.findMany({
      include: { dish: { include: { category: true } } },
      orderBy: { reviewed_at: 'desc' },
    });
    return reviews.map((r) => this.toResponse(r));
  }

  async findOne(id: string) {
    const review = await this.prisma.review.findUnique({
      where: { id },
      include: { dish: { include: { category: true } } },
    });
    if (!review) throw new NotFoundException('Review not found');
    return this.toResponse(review);
  }

  async findByDish(dishId: string) {
    const reviews = await this.prisma.review.findMany({
      where: { dish_id: dishId },
      include: { dish: { include: { category: true } } },
      orderBy: { reviewed_at: 'desc' },
    });
    return reviews.map((r) => this.toResponse(r));
  }

  async create(data: {
    dishId: string;
    rating: number;
    reviewText?: string;
    photoUrl?: string;
  }) {
    const dish = await this.prisma.dish.findUnique({
      where: { id: data.dishId },
    });
    if (!dish)
      throw new NotFoundException(`Dish not found for id: ${data.dishId}`);

    const review = await this.prisma.review.create({
      data: {
        dish_id: data.dishId,
        rating: data.rating,
        review_text: data.reviewText ?? null,
        photo_url: data.photoUrl ?? null,
      },
      include: { dish: { include: { category: true } } },
    });
    return this.toResponse(review);
  }

  async remove(id: string) {
    const review = await this.prisma.review.findUnique({ where: { id } });
    if (!review) throw new NotFoundException(`Review not found for id: ${id}`);
    await this.prisma.review.delete({ where: { id } });
  }

  private toResponse(review: ReviewWithRelations) {
    return {
      id: review.id,
      dishId: review.dish_id,
      dishName: review.dish?.name ?? '',
      category: review.dish?.category?.name ?? null,
      rating: Number(review.rating),
      reviewText: review.review_text,
      photoUrl: review.photo_url,
      reviewedAt: review.reviewed_at,
      createdAt: review.created_at,
    };
  }
}

import { Body, Controller, Delete, Get, Param, Post } from '@nestjs/common';
import { ReviewsService } from './reviews.service';

@Controller('api/reviews')
export class ReviewsController {
  constructor(private readonly reviewsService: ReviewsService) {}

  @Get()
  findAll() {
    return this.reviewsService.findAll();
  }

  @Get('dish/:dishId')
  findByDish(@Param('dishId') dishId: string) {
    return this.reviewsService.findByDish(dishId);
  }

  @Get(':id')
  findOne(@Param('id') id: string) {
    return this.reviewsService.findOne(id);
  }

  @Post()
  create(
    @Body()
    body: {
      dishId: string;
      rating: number;
      reviewText?: string;
      photoUrl?: string;
    },
  ) {
    return this.reviewsService.create(body);
  }

  @Delete(':id')
  remove(@Param('id') id: string) {
    return this.reviewsService.remove(id);
  }
}

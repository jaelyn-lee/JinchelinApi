import {
  Body,
  Controller,
  Delete,
  Get,
  Param,
  Post,
  Query,
} from '@nestjs/common';
import { DishesService } from './dishes.service';

@Controller('api/dishes')
export class DishesController {
  constructor(private readonly dishesService: DishesService) {}

  @Get()
  findAll(
    @Query('search') search?: string,
    @Query('categoryId') categoryId?: string,
    @Query('sortBy') sortBy?: string,
  ) {
    return this.dishesService.findAll(search, categoryId, sortBy);
  }

  @Get('ranking')
  getRanking() {
    return this.dishesService.getRanking();
  }

  @Get(':id')
  findOne(@Param('id') id: string) {
    return this.dishesService.findOne(id);
  }

  @Post()
  create(@Body() body: { name: string; categoryId?: string }) {
    return this.dishesService.create(body);
  }

  @Delete(':id')
  remove(@Param('id') id: string) {
    return this.dishesService.remove(id);
  }
}

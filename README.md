# HackerNews App

A simple HackerNews app that allows users to browse top stories and search for specific topics.

## What has been done

- Implemented endpoint that fetches top best stories from HackerNews API.
- Implemented caching using Redis
- Added Worker for getting data
- Added Unit tests

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Configuration](#configuration)


## Installation

1. Clone the repository:

git clone https://github.com/AlinaLadybug/HackerNews

2. Install redis and redisinsight

use docker-compose.yml file to pull and run redis and redisinsight images.

3. Run the app

## Usage

Once the app is running, you can use e.g. swagger or Postman to call an API.

## Potential Improvements

Here are some additional features that can be added to enhance the functionality of the HackerNews app:
1. **User Authentication**: Implement user authentication to allow users to log in, save favorite stories, and personalize their experience.

2. **Comments Section**: Integrate a comments section for each story, allowing users to read and contribute comments.

3. **Offline Support**: Implement offline support so that users can still browse previously loaded stories even without an internet connection.

4. **Sorting and Filtering**: Allow users to sort stories by different criteria (e.g., score, number of comments) and apply filters to narrow down the displayed stories.

5. **Performance Optimization**: Optimize the app's performance, especially when fetching and displaying large numbers of stories.

6. **Mapping**: Create additional DTO models to return original models in different way, use Automapper.

7. **Tests**: Cover more functionality with unit tests and also integration tests with hackernews API.

8. **Naming and structure**: Improve naming due to domain, and divide solution into different layers.

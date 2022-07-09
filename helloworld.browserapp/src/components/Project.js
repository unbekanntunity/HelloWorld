import React, { Component } from 'react';

import Header from '../components/Header';
import ImageSlider from './ImageSlider';

import VisibilitySensor from 'react-visibility-sensor';

import './Project.css';

class Project extends Component {
    state = {
        visibility: true
    }

    render() {
        return (
            <VisibilitySensor partialVisibility onChange={(isVisible) => isVisible && !this.props.creatorImage && this.props.onFirstAppear(this.props.keyProp)}>
                <div className="project-container" style={{
                        opacity: this.state.visibility ? 1 : 0.25,
                        transition: 'opacity 500ms linear',
                        width: this.props.width
                }} >
                    <Header title={this.props.title} tags={this.props.tags} members={this.props.members}
                        creatorImage={this.props.creatorImage} createdAt={this.props.createdAt}
                         onReportClick={this.props.onReportClick} />
                    <div className="description-container">
                        <p>{this.props.description}</p>
                    </div>
                    {
                        this.props.images !== undefined &&
                        <ImageSlider images={this.props.images} imageHeight={this.props.imageHeight} imageWidth={this.props.imageWidth} />
                    }
                </div>
            </VisibilitySensor>
        )
    }
}

export default Project;